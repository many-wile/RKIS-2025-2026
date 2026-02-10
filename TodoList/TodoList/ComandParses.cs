using System;
using System.Collections.Generic;
using TodoList.Commands;
namespace TodoList
{
	public static class CommandParser
	{
		private static Dictionary<string, Func<string, ICommand>> _commandHandlers = new Dictionary<string, Func<string, ICommand>>();
		static CommandParser()
		{
			_commandHandlers["add"] = ParseAdd;
			_commandHandlers["delete"] = ParseDelete;
			_commandHandlers["update"] = ParseUpdate;
			_commandHandlers["status"] = ParseStatus;
			_commandHandlers["view"] = ParseView;
			_commandHandlers["profile"] = ParseProfile;
			_commandHandlers["search"] = ParseSearch;
			_commandHandlers["help"] = (args) => new CommandHelp();
			_commandHandlers["undo"] = (args) => new UndoCommand();
			_commandHandlers["redo"] = (args) => new RedoCommand();
		}
		public static ICommand Parse(string inputString)
		{
			if (string.IsNullOrWhiteSpace(inputString))
				return null;
			var parts = inputString.Trim().Split(new char[] { ' ' }, 2);
			var commandName = parts[0].ToLower();
			var args = parts.Length > 1 ? parts[1] : "";
			if (_commandHandlers.TryGetValue(commandName, out var handler))
			{
				return handler(args);
			}
			Console.WriteLine($"Неизвестная команда: {commandName}");
			return new CommandHelp();
		}
		private static ICommand ParseSearch(string args)
		{
			return new SearchCommand(args.Trim());
		}
		private static ICommand ParseAdd(string args)
		{
			if (args.StartsWith("-m") || args.StartsWith("--multiline"))
			{
				Console.WriteLine("Введите задачу построчно. Для завершения '!end':");
				string text = "";
				while (true)
				{
					Console.Write("> ");
					string line = Console.ReadLine();
					if (line == "!end" || string.IsNullOrWhiteSpace(line))
						break;
					if (!string.IsNullOrEmpty(text)) text += "\n";
					text += line;
				}
				return new AddCommand(text);
			}
			return new AddCommand(args.Trim());
		}
		private static ICommand ParseDelete(string args)
		{
			if (int.TryParse(args, out int idx))
				return new DeleteCommand(idx);

			Console.WriteLine("Неверный формат аргумента для delete.");
			return null;
		}
		private static ICommand ParseUpdate(string args)
		{
			string[] parts = args.Split(' ', 2);
			if (parts.Length == 2 && int.TryParse(parts[0], out int idx))
				return new UpdateCommand(idx, parts[1]);

			Console.WriteLine("Неверный формат аргументов для update.");
			return null;
		}
		private static ICommand ParseStatus(string args)
		{
			string[] parts = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2 && int.TryParse(parts[0], out int idx))
			{
				if (Enum.TryParse<TodoStatus>(parts[1], true, out TodoStatus newStatus))
				{
					return new StatusCommand(idx, newStatus);
				}
				else
				{
					Console.WriteLine($"Неизвестный статус: '{parts[1]}'.");
					Console.WriteLine("Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed.");
					return null;
				}
			}
			Console.WriteLine("Неверный формат аргументов для status.");
			return null;
		}
		private static ICommand ParseView(string args)
		{
			ViewCommand command = new ViewCommand();
			string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			foreach (var part in parts)
			{
				switch (part)
				{
					case "-i":
					case "--index": command.ShowIndex = true; break;
					case "-s":
					case "--status": command.ShowDone = true; break;
					case "-d":
					case "--update-date": command.ShowDate = true; break;
					case "-a":
					case "--all": command.ShowAll = true; break;
				}
			}
			return command;
		}
		private static ICommand ParseProfile(string args)
		{
			return new ProfileCommand("profile " + args);
		}
	}
}