using System.Net.Http.Headers;

namespace TodoList;

public sealed class TodoApiClient : IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly bool _disposeClient;

	public TodoApiClient(string baseAddress = "http://localhost:5000/", HttpClient? httpClient = null)
	{
		if (httpClient == null)
		{
			_httpClient = new HttpClient();
			_disposeClient = true;
		}
		else
		{
			_httpClient = httpClient;
			_disposeClient = false;
		}

		_httpClient.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
		_httpClient.Timeout = TimeSpan.FromSeconds(5);
	}

	public async Task UploadProfilesAsync(byte[] encryptedPayload, CancellationToken cancellationToken = default)
	{
		using var content = CreateBinaryContent(encryptedPayload);
		using HttpResponseMessage response = await _httpClient.PostAsync("profiles", content, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task<byte[]> DownloadProfilesAsync(CancellationToken cancellationToken = default)
	{
		using HttpResponseMessage response = await _httpClient.GetAsync("profiles", cancellationToken);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsByteArrayAsync(cancellationToken);
	}

	public async Task UploadTodosAsync(Guid userId, byte[] encryptedPayload, CancellationToken cancellationToken = default)
	{
		using var content = CreateBinaryContent(encryptedPayload);
		using HttpResponseMessage response = await _httpClient.PostAsync($"todos/{userId}", content, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task<byte[]> DownloadTodosAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		using HttpResponseMessage response = await _httpClient.GetAsync($"todos/{userId}", cancellationToken);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsByteArrayAsync(cancellationToken);
	}

	public async Task<bool> IsServerAvailableAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			using HttpResponseMessage response = await _httpClient.GetAsync("profiles", cancellationToken);
			return response.IsSuccessStatusCode;
		}
		catch
		{
			return false;
		}
	}

	public void Dispose()
	{
		if (_disposeClient)
		{
			_httpClient.Dispose();
		}
	}

	private static ByteArrayContent CreateBinaryContent(byte[] encryptedPayload)
	{
		var payload = encryptedPayload ?? Array.Empty<byte>();
		var content = new ByteArrayContent(payload);
		content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
		return content;
	}
}