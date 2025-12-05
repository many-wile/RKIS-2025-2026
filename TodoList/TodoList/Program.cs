using TodoList.Commands;
using System;
using System.IO;
using System.Linq; 
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
			while (AppInfo.CurrentProfile == null)
			{
				Console.WriteLine("\n1 - Войти");
				Console.WriteLine("2 - Зарегистрироваться");
				Console.WriteLine("exit - Выход");
				Console.Write("> ");
				string choice = Console.ReadLine();
				switch (choice)
				{
					case "1":
						Login();
						break;
					case "2":
						Register();
						break;
					case "exit":
						return;
					default:
						Console.WriteLine("Неверный выбор.");
						break;
				}
			}
			string userTodosPath = Path.Combine(DataDirectory, $"todos_{AppInfo.CurrentProfile.Login}.csv");
			AppInfo.Todos = FileManager.LoadTodos(userTodosPath);
			Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.FirstName}! Введите команду (help — для списка команд):");
			while (true)
			{
				Console.Write("> ");
				string input = Console.ReadLine()?.Trim();

				if (string.IsNullOrWhiteSpace(input))
					continue;
				if (input.ToLower() == "exit")
				{
					FileManager.SaveTodos(AppInfo.Todos, userTodosPath);
					break;
				}
				ICommand command = CommandParser.Parse(input);
				if (command != null)
				{
					command.Execute();
					if (command is AddCommand || command is DeleteCommand || command is UpdateCommand || command is StatusCommand)
					{
						AppInfo.UndoStack.Push(command);
						AppInfo.RedoStack.Clear();
						FileManager.SaveTodos(AppInfo.Todos, userTodosPath);
					}
					else if (command is UndoCommand || command is RedoCommand)
					{
						FileManager.SaveTodos(AppInfo.Todos, userTodosPath);
					}
				}
			}
			Console.WriteLine("Программа завершена.");
		}
		static void Login()
		{
			Console.Write("Логин: ");
			string login = Console.ReadLine();
			Console.Write("Пароль: ");
			string password = Console.ReadLine();
			Profile foundProfile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

			if (foundProfile != null && foundProfile.Password == password)
			{
				AppInfo.CurrentProfileId = foundProfile.Id;
				Console.WriteLine("Вход выполнен успешно!");
			}
			else
			{
				Console.WriteLine("Неверный логин или пароль.");
			}
		}
		static void Register()
		{
			Console.Write("Имя: ");
			string firstName = Console.ReadLine();
			Console.Write("Фамилия: ");
			string lastName = Console.ReadLine();
			Console.Write("Год рождения: ");
			if (!int.TryParse(Console.ReadLine(), out int birthYear) || birthYear < 1900 || birthYear > DateTime.Now.Year)
			{
				Console.WriteLine("Некорректный год рождения.");
				return;
			}
			Console.Write("Логин: ");
			string login = Console.ReadLine();
			if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
			{
				Console.WriteLine("Пользователь с таким логином уже существует.");
				return;
			}
			Console.Write("Пароль: ");
			string password = Console.ReadLine();
			var newProfile = new Profile(firstName, lastName, birthYear, login, password);
			AppInfo.AllProfiles.Add(newProfile);
			FileManager.SaveProfiles(AppInfo.AllProfiles, Path.Combine(DataDirectory, ProfilesFileName));
			AppInfo.CurrentProfileId = newProfile.Id;
			Console.WriteLine("Регистрация прошла успешно! Вы вошли в систему.");
		}
	}
}