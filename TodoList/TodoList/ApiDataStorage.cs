using System.Net.Http;
using System.Text.Json;
using TodoList.Exceptions;

namespace TodoList;

public sealed class ApiDataStorage : IDataStorage, IDisposable
{
	private readonly TodoApiClient _apiClient;
	private readonly bool _disposeClient;
	private readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public ApiDataStorage(TodoApiClient? apiClient = null)
	{
		if (apiClient == null)
		{
			_apiClient = new TodoApiClient();
			_disposeClient = true;
		}
		else
		{
			_apiClient = apiClient;
			_disposeClient = false;
		}
	}

	public void SaveProfiles(IEnumerable<Profile> profiles)
	{
		if (profiles == null)
		{
			return;
		}

		RunWithStorageHandling(() =>
		{
			List<ProfileDto> dto = profiles
				.Select(MapProfileToDto)
				.ToList();

			string json = JsonSerializer.Serialize(dto, _jsonOptions);
			byte[] encrypted = PayloadCrypto.EncryptString(json);
			_apiClient.UploadProfilesAsync(encrypted).GetAwaiter().GetResult();
		});
	}

	public IEnumerable<Profile> LoadProfiles()
	{
		return RunWithStorageHandling(() =>
		{
			byte[] encrypted = _apiClient.DownloadProfilesAsync().GetAwaiter().GetResult();
			if (encrypted.Length == 0)
			{
				return new List<Profile>();
			}

			string json = PayloadCrypto.DecryptToString(encrypted);
			List<ProfileDto> dto = JsonSerializer.Deserialize<List<ProfileDto>>(json, _jsonOptions) ?? new List<ProfileDto>();

			return dto.Select(MapDtoToProfile).ToList();
		});
	}

	public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
	{
		if (todos == null)
		{
			return;
		}

		RunWithStorageHandling(() =>
		{
			List<TodoItemDto> dto = todos
				.Select(MapTodoToDto)
				.ToList();

			string json = JsonSerializer.Serialize(dto, _jsonOptions);
			byte[] encrypted = PayloadCrypto.EncryptString(json);
			_apiClient.UploadTodosAsync(userId, encrypted).GetAwaiter().GetResult();
		});
	}

	public IEnumerable<TodoItem> LoadTodos(Guid userId)
	{
		return RunWithStorageHandling(() =>
		{
			byte[] encrypted = _apiClient.DownloadTodosAsync(userId).GetAwaiter().GetResult();
			if (encrypted.Length == 0)
			{
				return new List<TodoItem>();
			}

			string json = PayloadCrypto.DecryptToString(encrypted);
			List<TodoItemDto> dto = JsonSerializer.Deserialize<List<TodoItemDto>>(json, _jsonOptions) ?? new List<TodoItemDto>();

			return dto.Select(MapDtoToTodo).ToList();
		});
	}

	public bool IsServerAvailable()
	{
		return _apiClient.IsServerAvailableAsync().GetAwaiter().GetResult();
	}

	public void Dispose()
	{
		if (_disposeClient)
		{
			_apiClient.Dispose();
		}
	}

	private static ProfileDto MapProfileToDto(Profile profile)
	{
		return new ProfileDto
		{
			Id = profile.Id,
			Login = profile.Login,
			Password = profile.Password,
			FirstName = profile.FirstName,
			LastName = profile.LastName,
			BirthYear = profile.BirthYear
		};
	}

	private static Profile MapDtoToProfile(ProfileDto dto)
	{
		return new Profile(dto.Id, dto.Login, dto.Password, dto.FirstName, dto.LastName, dto.BirthYear);
	}

	private static TodoItemDto MapTodoToDto(TodoItem todo)
	{
		return new TodoItemDto
		{
			Text = todo.Text,
			Status = todo.Status,
			LastUpdate = todo.LastUpdate
		};
	}

	private static TodoItem MapDtoToTodo(TodoItemDto dto)
	{
		return new TodoItem(dto.Text, dto.Status, dto.LastUpdate);
	}

	private static void RunWithStorageHandling(Action action)
	{
		try
		{
			action();
		}
		catch (System.Security.Cryptography.CryptographicException ex)
		{
			throw new DataEncryptionException("Encryption/decryption error during API storage operation.", ex);
		}
		catch (HttpRequestException ex)
		{
			throw new DataAccessException("Cannot connect to API server.", ex);
		}
		catch (TaskCanceledException ex)
		{
			throw new DataAccessException("Request to API server timed out.", ex);
		}
		catch (JsonException ex)
		{
			throw new DataCorruptionException("Invalid JSON payload from API server.", ex);
		}
	}

	private static T RunWithStorageHandling<T>(Func<T> action)
	{
		try
		{
			return action();
		}
		catch (System.Security.Cryptography.CryptographicException ex)
		{
			throw new DataEncryptionException("Encryption/decryption error during API storage operation.", ex);
		}
		catch (HttpRequestException ex)
		{
			throw new DataAccessException("Cannot connect to API server.", ex);
		}
		catch (TaskCanceledException ex)
		{
			throw new DataAccessException("Request to API server timed out.", ex);
		}
		catch (JsonException ex)
		{
			throw new DataCorruptionException("Invalid JSON payload from API server.", ex);
		}
	}

	private sealed class ProfileDto
	{
		public Guid Id { get; set; }
		public string Login { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public int BirthYear { get; set; }
	}

	private sealed class TodoItemDto
	{
		public string Text { get; set; } = string.Empty;
		public TodoStatus Status { get; set; }
		public DateTime LastUpdate { get; set; }
	}
}
