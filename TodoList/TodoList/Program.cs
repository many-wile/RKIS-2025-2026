using System;
using System.Collections.Generic;
using TodoList.Commands;
using TodoList.Exceptions;

namespace TodoList
{
	class Program
	{
		private const string ProfilesPath = "Data/profiles.csv";

		static void Main(string[] args)
		{
			Console.WriteLine("СДЕЛАНО by НЕСТЕРЕНКО ГОРЕЛОВ");

			// 1. Убеждаемся, что папка Data существует
			FileManager.EnsureDataDirectory("Data");

			// 2. Загружаем все профили перед стартом 
			ProfileManager.LoadProfiles(ProfilesPath);

			// ИСПРАВЛЕНИЕ: Связываем списки профилей между собой!
			AppInfo.AllProfiles = ProfileManager.AllProfiles;

			// 3. Запускаем логику авторизации с вопросом y/n
			AuthenticateUser();

			Console.WriteLine("\nTodoList запущен. Введите 'help' для списка команд.");
			while (true)
			{
				Console.Write("> ");
				string input = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(input)) continue;
				try
				{
					ICommand command = ParseCommand(input);
					if (command != null)
					{
						command.Execute();

						// Добавление команды в стек для Undo (если она поддерживает отмену)
						if (command is IUndo && !(command is UndoCommand) && !(command is RedoCommand))
						{
							AppInfo.UndoStack.Push(command);
							AppInfo.RedoStack.Clear();
						}
					}
				}
				catch (TaskNotFoundException ex) { Console.WriteLine($"Ошибка задачи: {ex.Message}"); }
				catch (AuthenticationException ex) { Console.WriteLine($"Ошибка авторизации: {ex.Message}"); }
				catch (InvalidCommandException ex) { Console.WriteLine($"Ошибка команды: {ex.Message}"); }
				catch (InvalidArgumentException ex) { Console.WriteLine($"Ошибка аргументов: {ex.Message}"); }
				catch (Exception ex) { Console.WriteLine($"Неожиданная ошибка: {ex.Message}"); }
			}
		}

		private static void AuthenticateUser()
		{
			while (AppInfo.CurrentProfileId == null)
			{
				Console.Write("\nУ вас уже есть аккаунт? (y/n): ");
				string choice = Console.ReadLine()?.Trim().ToLower();

				if (choice == "y" || choice == "н")
				{
					Console.WriteLine("=== ВХОД В ПРОФИЛЬ ===");
					Console.Write("Введите логин: ");
					string login = Console.ReadLine();
					Console.Write("Введите пароль: ");
					string password = Console.ReadLine();

					Profile profile = ProfileManager.FindByLogin(login);
					if (profile != null && profile.Password == password)
					{
						// Устанавливаем текущего пользователя
						AppInfo.CurrentProfileId = profile.Id;
						Console.WriteLine($"Успешный вход! Добро пожаловать, {profile.FirstName}.");

						// Подгружаем зашифрованные задачи пользователя после успешного входа
						var loadedTodos = FileManager.LoadTodos(AppInfo.CurrentUserTodosPath);
						AppInfo.AllTodos[profile.Id] = loadedTodos;
					}
					else
					{
						Console.WriteLine("Ошибка: Неверный логин или пароль.");
					}
				}
				else if (choice == "n" || choice == "т")
				{
					Console.WriteLine("=== РЕГИСТРАЦИЯ ===");
					Console.Write("Введите имя: ");
					string firstName = Console.ReadLine();
					Console.Write("Введите фамилию: ");
					string lastName = Console.ReadLine();

					// ИСПРАВЛЕНИЕ: Блокировка ввода букв вместо года рождения
					Console.Write("Введите год рождения (цифрами): ");
					int birthYear;
					while (!int.TryParse(Console.ReadLine(), out birthYear))
					{
						Console.Write("Ошибка! Год должен состоять только из цифр. Повторите ввод: ");
					}

					Console.Write("Придумайте логин: ");
					string login = Console.ReadLine();
					Console.Write("Придумайте пароль: ");
					string password = Console.ReadLine();

					if (ProfileManager.FindByLogin(login) != null)
					{
						Console.WriteLine("Ошибка: Пользователь с таким логином уже существует.");
					}
					else
					{
						Profile newProfile = new Profile(firstName, lastName, birthYear, login, password);
						ProfileManager.AddProfile(newProfile);
						ProfileManager.SaveProfiles(ProfilesPath); // Сохраняем (и сразу шифруем)

						Console.WriteLine("Профиль успешно создан! Теперь вы можете войти.");
					}
				}
				else
				{
					Console.WriteLine("Неизвестный ввод. Пожалуйста, введите 'y' (да) или 'n' (нет).");
				}
			}
		}

		private static ICommand ParseCommand(string input)
		{
			var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 0) throw new InvalidCommandException("Пустая команда.");

			string commandName = parts[0].ToLower();
			string args = parts.Length > 1 ? input.Substring(commandName.Length).Trim() : "";
			switch (commandName)
			{
				case "add": return new AddCommand(args);
				case "view": return new ViewCommand { ShowAll = args.Contains("-a") };
				case "delete":
					if (int.TryParse(args, out int delIndex)) return new DeleteCommand(delIndex);
					throw new InvalidArgumentException("Неверный формат индекса для delete.");
				case "update":
					var updateParts = args.Split(' ', 2);
					if (updateParts.Length < 2 || !int.TryParse(updateParts[0], out int upIndex))
						throw new InvalidArgumentException("Использование: update <index> <text>");
					return new UpdateCommand(upIndex, updateParts[1]);
				case "status":
					var stParts = args.Split(' ', 2);
					if (stParts.Length < 2 || !int.TryParse(stParts[0], out int stIndex) || !Enum.TryParse(stParts[1], true, out TodoStatus status))
						throw new InvalidArgumentException("Использование: status <index> <Status>");
					return new StatusCommand(stIndex, status);
				case "search": return new SearchCommand(args);
				case "undo": return new UndoCommand();
				case "redo": return new RedoCommand();
				case "profile": return new ProfileCommand(input);
				case "help": return new CommandHelp();
				case "load": return new LoadCommand(args);
				case "exit":
					Environment.Exit(0);
					return null;
				default:
					throw new InvalidCommandException($"Команда '{commandName}' не найдена.");
			}
		}
	}
}