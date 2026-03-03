using System;
using System.Collections.Generic;
using TodoList.Commands;
using TodoList.Exceptions;
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
			_commandHandlers["load"] = ParseLoad;
		}
		public static ICommand Parse(string inputString)
		{
			if (string.IsNullOrWhiteSpace(inputString)) return null;
			var parts = inputString.Trim().Split(new char[] { ' ' }, 2);
			var commandName = parts[0].ToLower();
			var args = parts.Length > 1 ? parts[1] : "";
			if (_commandHandlers.TryGetValue(commandName, out var handler))
			{
				return handler(args);
			}
			throw new InvalidCommandException($"Команда '{commandName}' не найдена. Введите 'help' для списка.");
		}
		private static ICommand ParseSearch(string args) => new SearchCommand(args.Trim());
		private static ICommand ParseAdd(string args)
		{
			if (args.StartsWith("-m"))
			{
				Console.WriteLine("Введите задачу (!end для завершения):");
				string text = "";
				while (true)
				{
					Console.Write("> ");
					string line = Console.ReadLine();
					if (line == "!end") break;
					text += line + "\n";
				}
				if (string.IsNullOrWhiteSpace(text))
					throw new InvalidArgumentException("Текст задачи не может быть пустым.");
				return new AddCommand(text.Trim());
			}
			if (string.IsNullOrWhiteSpace(args))
				throw new InvalidArgumentException("Введите текст задачи после команды add.");

			return new AddCommand(args.Trim());
		}
		private static ICommand ParseDelete(string args)
		{
			if (int.TryParse(args, out int idx))
				return new DeleteCommand(idx);

			throw new InvalidArgumentException("Укажите числовой номер задачи. Пример: delete 1");
		}
		private static ICommand ParseUpdate(string args)
		{
			string[] parts = args.Split(new char[] { ' ' }, 2);
			if (parts.Length == 2 && int.TryParse(parts[0], out int idx))
			{
				if (string.IsNullOrWhiteSpace(parts[1]))
					throw new InvalidArgumentException("Новый текст задачи не может быть пустым.");

				return new UpdateCommand(idx, parts[1]);
			}
			throw new InvalidArgumentException("Неверный формат. Используйте: update <номер> <новый текст>");
		}
		private static ICommand ParseStatus(string args)
		{
			string[] parts = args.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2 && int.TryParse(parts[0], out int idx))
			{
				if (Enum.TryParse<TodoStatus>(parts[1], true, out TodoStatus newStatus))
				{
					return new StatusCommand(idx, newStatus);
				}
				else
				{
					throw new InvalidArgumentException($"Статус '{parts[1]}' не найден. Доступные: NotStarted, InProgress, Completed.");
				}
			}
			throw new InvalidArgumentException("Неверный формат. Используйте: status <номер> <статус>");
		}
		private static ICommand ParseView(string args)
		{
			ViewCommand command = new ViewCommand();
			string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var part in parts)
			{
				switch (part.ToLower())
				{
					case "-i": case "--index": command.ShowIndex = true; break;
					case "-s": case "--status": command.ShowDone = true; break;
					case "-d": case "--update-date": command.ShowDate = true; break;
					case "-a": case "--all": command.ShowAll = true; break;
					default: throw new InvalidArgumentException($"Неизвестный флаг: {part}");
				}
			}
			return command;
		}
		private static ICommand ParseProfile(string args) => new ProfileCommand("profile " + args);
		private static ICommand ParseLoad(string args) => new LoadCommand(args);
	}
}