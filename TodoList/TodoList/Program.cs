using TodoList.Commands;
using System;
using System.Globalization;
using System.IO;
namespace TodoList
{
	internal class Program
	{
		private const string DataDirectory = "Data";
		private const string ProfileFileName = "profile.txt";
		private const string TodosFileName = "todos.csv";
		static void Main(string[] args)
		{
			Console.WriteLine("Работу выполнили Нестеренко и Горелов");
			FileManager.EnsureDataDirectory(DataDirectory);
			Profile profile = LoadOrCreateProfile(Path.Combine(DataDirectory, ProfileFileName));
			if (profile == null)
			{
				return;
			}
			TodoList todoList = FileManager.LoadTodos(Path.Combine(DataDirectory, TodosFileName));
			if (todoList == null)
			{
				todoList = new TodoList();
			}
			Console.WriteLine("Введите команду (help — для списка команд):");
			while (true)
			{
				Console.Write("> ");
				string input = Console.ReadLine()?.Trim();

				if (string.IsNullOrWhiteSpace(input))
					continue;
				if (input.ToLower() == "exit")
				{
					FileManager.SaveTodos(todoList, Path.Combine(DataDirectory, TodosFileName));
					break;
				}
				ICommand command = CommandParser.Parse(input, todoList, profile);
				if (command != null)
				{
					command.Execute();
					if (command is AddCommand || command is DeleteCommand || command is UpdateCommand || command is StatusCommand)
					{
						FileManager.SaveTodos(todoList, Path.Combine(DataDirectory, TodosFileName));
					}
				}
				else
				{
					if (input.ToLower() != "help")
					{
						Console.WriteLine("Неизвестная команда. Введите 'help' для списка команд.");
					}
				}
			}
			Console.WriteLine("Программа завершена.");
		}
		static Profile LoadOrCreateProfile(string filePath)
		{
			Profile profile = FileManager.LoadProfile(filePath);
			if (profile == null)
			{
				Console.WriteLine("Введите ваше полное имя:");
				string fullName = Console.ReadLine();
				Console.WriteLine("Введите дату рождения (ДД.MM.ГГГГ):");
				string dateStr = Console.ReadLine();
				if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
				{
					Console.WriteLine("Неверный формат даты! Профиль не будет создан.");
					return null;
				}
				profile = new Profile(fullName, birthDate);
				FileManager.SaveProfile(profile, filePath);
			}
			return profile;
		}
	}
}