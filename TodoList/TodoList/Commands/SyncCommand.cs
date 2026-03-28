using TodoList.Exceptions;

namespace TodoList.Commands;

internal class SyncCommand : ICommand
{
	private readonly string _args;
	private readonly IDataStorage _localStorage;
	private readonly ApiDataStorage _apiStorage;

	private bool _pull;
	private bool _push;

	public SyncCommand(string args, IDataStorage localStorage, ApiDataStorage apiStorage)
	{
		_args = args ?? string.Empty;
		_localStorage = localStorage;
		_apiStorage = apiStorage;
	}

	public void Execute()
	{
		ParseFlags();

		if (!_apiStorage.IsServerAvailable())
		{
			Console.WriteLine("Ошибка: сервер недоступен.");
			return;
		}

		if (_pull)
		{
			PullFromServer();
			Console.WriteLine("Синхронизация завершена: данные загружены с сервера.");
		}

		if (_push)
		{
			PushToServer();
			Console.WriteLine("Синхронизация завершена: данные отправлены на сервер.");
		}
	}

	private void ParseFlags()
	{
		string[] flags = _args
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		foreach (string flag in flags)
		{
			switch (flag.ToLowerInvariant())
			{
				case "--pull":
				case "-pull":
					_pull = true;
					break;
				case "--push":
				case "-push":
					_push = true;
					break;
				default:
					throw new InvalidArgumentException($"Неизвестный флаг sync: {flag}");
			}
		}

		if (!_pull && !_push)
		{
			throw new InvalidArgumentException("Использование: sync --pull или sync --push");
		}

		if (_pull && _push)
		{
			throw new InvalidArgumentException("Выберите только один флаг: --pull или --push");
		}
	}

	private void PullFromServer()
	{
		List<Profile> profiles = _apiStorage.LoadProfiles().ToList();

		ProfileManager.AllProfiles = profiles;
		AppInfo.AllProfiles = profiles;
		_localStorage.SaveProfiles(profiles);

		AppInfo.AllTodos.Clear();
		foreach (Profile profile in profiles)
		{
			List<TodoItem> todos = _apiStorage.LoadTodos(profile.Id).ToList();
			AppInfo.AllTodos[profile.Id] = new TodoList(todos);
			_localStorage.SaveTodos(profile.Id, todos);
		}

		if (AppInfo.CurrentProfileId.HasValue)
		{
			bool profileExists = profiles.Any(p => p.Id == AppInfo.CurrentProfileId.Value);
			if (!profileExists)
			{
				AppInfo.CurrentProfileId = null;
				Console.WriteLine("Текущий пользователь отсутствует в данных сервера. Выполнен выход.");
			}
		}
	}

	private void PushToServer()
	{
		List<Profile> profiles = ProfileManager.AllProfiles;
		_apiStorage.SaveProfiles(profiles);

		foreach (Profile profile in profiles)
		{
			List<TodoItem> todos;
			if (AppInfo.AllTodos.TryGetValue(profile.Id, out TodoList? todoList))
			{
				todos = todoList.ToList();
			}
			else
			{
				todos = _localStorage.LoadTodos(profile.Id).ToList();
			}

			_apiStorage.SaveTodos(profile.Id, todos);
		}
	}
}