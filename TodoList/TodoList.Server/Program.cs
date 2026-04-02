using System.Net;

namespace TodoList.Server;

internal static class Program
{
	private const string Prefix = "http://localhost:5000/";
	private static readonly string DataDirectory = Path.Combine(AppContext.BaseDirectory, "ServerData");
	private static readonly string ProfilesPath = Path.Combine(DataDirectory, "profiles.dat");

	private static async Task Main(string[] args)
	{
		Directory.CreateDirectory(DataDirectory);

		using var listener = new HttpListener();
		listener.Prefixes.Add(Prefix);
		listener.Start();

		Console.WriteLine($"TodoList.Server is running on {Prefix}");

		while (true)
		{
			HttpListenerContext context = await listener.GetContextAsync();
			_ = Task.Run(() => HandleRequestAsync(context));
		}
	}

	private static async Task HandleRequestAsync(HttpListenerContext context)
	{
		HttpListenerRequest request = context.Request;
		HttpListenerResponse response = context.Response;

		try
		{
			if (request.HttpMethod == "POST" && IsProfilesPath(request.Url))
			{
				byte[] encryptedPayload = await ReadRequestBodyAsync(request);
				await File.WriteAllBytesAsync(ProfilesPath, encryptedPayload);
				await WriteBinaryResponseAsync(response, Array.Empty<byte>(), HttpStatusCode.OK);
				return;
			}

			if (request.HttpMethod == "GET" && IsProfilesPath(request.Url))
			{
				byte[] encryptedPayload = await ReadFileOrEmptyAsync(ProfilesPath);
				await WriteBinaryResponseAsync(response, encryptedPayload, HttpStatusCode.OK);
				return;
			}

			if (request.HttpMethod == "POST" && TryGetTodosUserId(request.Url, out Guid postUserId))
			{
				string todosPath = BuildTodosPath(postUserId);
				byte[] encryptedPayload = await ReadRequestBodyAsync(request);
				await File.WriteAllBytesAsync(todosPath, encryptedPayload);
				await WriteBinaryResponseAsync(response, Array.Empty<byte>(), HttpStatusCode.OK);
				return;
			}

			if (request.HttpMethod == "GET" && TryGetTodosUserId(request.Url, out Guid getUserId))
			{
				string todosPath = BuildTodosPath(getUserId);
				byte[] encryptedPayload = await ReadFileOrEmptyAsync(todosPath);
				await WriteBinaryResponseAsync(response, encryptedPayload, HttpStatusCode.OK);
				return;
			}

			response.StatusCode = (int)HttpStatusCode.NotFound;
			await WriteTextResponseAsync(response, "Endpoint not found.");
		}
		catch (Exception ex)
		{
			response.StatusCode = (int)HttpStatusCode.InternalServerError;
			await WriteTextResponseAsync(response, $"Server error: {ex.Message}");
		}
		finally
		{
			response.Close();
		}
	}

	private static bool IsProfilesPath(Uri? uri)
	{
		if (uri == null)
		{
			return false;
		}

		return uri.AbsolutePath.Equals("/profiles", StringComparison.OrdinalIgnoreCase);
	}

	private static bool TryGetTodosUserId(Uri? uri, out Guid userId)
	{
		userId = Guid.Empty;
		if (uri == null)
		{
			return false;
		}

		string[] parts = uri.AbsolutePath
			.Trim('/')
			.Split('/', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2)
		{
			return false;
		}

		return parts[0].Equals("todos", StringComparison.OrdinalIgnoreCase)
			&& Guid.TryParse(parts[1], out userId);
	}

	private static string BuildTodosPath(Guid userId)
	{
		return Path.Combine(DataDirectory, $"todos_{userId}.dat");
	}

	private static async Task<byte[]> ReadRequestBodyAsync(HttpListenerRequest request)
	{
		using var memoryStream = new MemoryStream();
		await request.InputStream.CopyToAsync(memoryStream);
		return memoryStream.ToArray();
	}

	private static async Task<byte[]> ReadFileOrEmptyAsync(string path)
	{
		if (!File.Exists(path))
		{
			return Array.Empty<byte>();
		}

		return await File.ReadAllBytesAsync(path);
	}

	private static async Task WriteBinaryResponseAsync(HttpListenerResponse response, byte[] payload, HttpStatusCode statusCode)
	{
		response.StatusCode = (int)statusCode;
		response.ContentType = "application/octet-stream";
		response.ContentLength64 = payload.LongLength;
		await response.OutputStream.WriteAsync(payload, 0, payload.Length);
	}

	private static async Task WriteTextResponseAsync(HttpListenerResponse response, string message)
	{
		byte[] payload = System.Text.Encoding.UTF8.GetBytes(message);
		response.ContentType = "text/plain; charset=utf-8";
		response.ContentLength64 = payload.LongLength;
		await response.OutputStream.WriteAsync(payload, 0, payload.Length);
	}
}
