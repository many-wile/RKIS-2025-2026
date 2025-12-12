using System;
using System.IO;
using System.Linq;
using TodoList.Commands;
namespace TodoList
{
	internal class Program
	{
		private const string DataDirectory = "Data";
		private const string ProfilesFileName = "profiles.csv";
		static void Main(string[] args)
		{
			Console.WriteLine("Работу выполнили Нестеренко и Горелов");
			FileManager.EnsureDataDirectory(DataDirectory);
			AppInfo.AllProfiles = FileManager.LoadProfiles(Path.Combine(DataDirectory, ProfilesFileName));
			while (true)
			{
				while (AppInfo.CurrentProfile == null)
				{
					Console.Write("\nВойти в существующий профиль? [y/n] (или 'exit' для выхода): ");
					string choice = Console.ReadLine()?.ToLower();
					if (choice == "y")
					{
						Login();
					}
					else if (choice == "n")
					{
						Register();
					}
					else if (choice == "exit")
					{
						Console.WriteLine("Программа завершена.");
						return;
					}
					else
					{
						Console.WriteLine("Неверный ввод. Пожалуйста, введите 'y' или 'n'.");
					}
				}
				string userTodosPath = Path.Combine(DataDirectory, $"todos_{AppInfo.CurrentProfile.Id}.csv");
				Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.FirstName}! Введите команду (help — для списка команд):");
				while (AppInfo.CurrentProfile != null)
				{
					Console.Write("> ");
					string input = Console.ReadLine()?.Trim();
					if (string.IsNullOrWhiteSpace(input))
						continue;
					if (input.ToLower() == "exit")
					{
						Console.WriteLine("Программа завершена.");
						return;
					}
					ICommand command = CommandParser.Parse(input);
					if (command != null)
					{
						command.Execute();
						if (command is AddCommand || command is DeleteCommand || command is UpdateCommand || command is StatusCommand)
						{
							AppInfo.UndoStack.Push(command);
							AppInfo.RedoStack.Clear();
						}
					}
				}
			}
		}
		static void Login()
		{
			Console.Write("Введите логин: ");
			string login = Console.ReadLine();
			Console.Write("Введите пароль: ");
			string password = Console.ReadLine();
			Profile foundProfile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
			if (foundProfile != null && foundProfile.Password == password)
			{
				AppInfo.CurrentProfileId = foundProfile.Id;
				string userTodosPath = Path.Combine(DataDirectory, $"todos_{foundProfile.Id}.csv");
				TodoList todos = FileManager.LoadTodos(userTodosPath);
				Action<TodoItem> saveHandler = (item) => FileManager.SaveTodos(todos, userTodosPath);
				todos.OnTodoAdded += saveHandler;
				todos.OnTodoDeleted += saveHandler;
				todos.OnTodoUpdated += saveHandler;
				todos.OnStatusChanged += saveHandler;
				AppInfo.AllTodos[foundProfile.Id] = todos;
				AppInfo.UndoStack.Clear();
				AppInfo.RedoStack.Clear();
				Console.WriteLine("Вход выполнен успешно!");
			}
			else
			{
				Console.WriteLine("Неверный логин или пароль.");
			}
		}
		static void Register()
		{
			Console.WriteLine("\n--- Создание нового профиля ---");
			Console.Write("Введите имя: ");
			string firstName = Console.ReadLine();
			Console.Write("Введите фамилию: ");
			string lastName = Console.ReadLine();
			Console.Write("Введите год рождения: ");
			if (!int.TryParse(Console.ReadLine(), out int birthYear) || birthYear < 1900 || birthYear > DateTime.Now.Year)
			{
				Console.WriteLine("Некорректный год рождения. Регистрация отменена.");
				return;
			}
			Console.Write("Введите логин: ");
			string login = Console.ReadLine();
			if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
			{
				Console.WriteLine("Пользователь с таким логином уже существует. Регистрация отменена.");
				return;
			}
			Console.Write("Введите пароль: ");
			string password = Console.ReadLine();
			var newProfile = new Profile(firstName, lastName, birthYear, login, password);
			AppInfo.AllProfiles.Add(newProfile);
			FileManager.SaveProfiles(AppInfo.AllProfiles, Path.Combine(DataDirectory, ProfilesFileName));
			AppInfo.CurrentProfileId = newProfile.Id;
			TodoList newTodos = new TodoList();
			string userTodosPath = Path.Combine(DataDirectory, $"todos_{newProfile.Id}.csv");
			Action<TodoItem> saveHandler = (item) => FileManager.SaveTodos(newTodos, userTodosPath);
			newTodos.OnTodoAdded += saveHandler;
			newTodos.OnTodoDeleted += saveHandler;
			newTodos.OnTodoUpdated += saveHandler;
			newTodos.OnStatusChanged += saveHandler;
			AppInfo.AllTodos[newProfile.Id] = newTodos;
			AppInfo.UndoStack.Clear();
			AppInfo.RedoStack.Clear();
			Console.WriteLine("Регистрация прошла успешно! Вы вошли в систему.");
		}
	}
}