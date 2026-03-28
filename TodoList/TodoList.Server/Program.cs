using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

string dataDirectory = Path.Combine(AppContext.BaseDirectory, "ServerData");
string profilesPath = Path.Combine(dataDirectory, "profiles.dat");
Directory.CreateDirectory(dataDirectory);

app.MapPost("/profiles", async (HttpRequest request) =>
{
	byte[] encryptedPayload = await ReadRequestBodyAsync(request);
	await File.WriteAllBytesAsync(profilesPath, encryptedPayload);
	return Results.Ok();
});

app.MapGet("/profiles", async () =>
{
	byte[] encryptedPayload = await ReadFileOrEmptyAsync(profilesPath);
	return Results.File(encryptedPayload, "application/octet-stream");
});

app.MapPost("/todos/{userId:guid}", async (Guid userId, HttpRequest request) =>
{
	string todosPath = BuildTodosPath(dataDirectory, userId);
	byte[] encryptedPayload = await ReadRequestBodyAsync(request);
	await File.WriteAllBytesAsync(todosPath, encryptedPayload);
	return Results.Ok();
});

app.MapGet("/todos/{userId:guid}", async (Guid userId) =>
{
	string todosPath = BuildTodosPath(dataDirectory, userId);
	byte[] encryptedPayload = await ReadFileOrEmptyAsync(todosPath);
	return Results.File(encryptedPayload, "application/octet-stream");
});

app.Run();

static string BuildTodosPath(string dataDirectory, Guid userId)
{
	return Path.Combine(dataDirectory, $"todos_{userId}.dat");
}

static async Task<byte[]> ReadRequestBodyAsync(HttpRequest request)
{
	using var memoryStream = new MemoryStream();
	await request.Body.CopyToAsync(memoryStream);
	return memoryStream.ToArray();
}

static async Task<byte[]> ReadFileOrEmptyAsync(string path)
{
	if (!File.Exists(path))
	{
		return Array.Empty<byte>();
	}

	return await File.ReadAllBytesAsync(path);
}
