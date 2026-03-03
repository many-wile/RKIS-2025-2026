using System;
using System.Collections.Generic;
using TodoList.Commands;
using TodoList.Exceptions;
namespace TodoList
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("TodoList запущен. Введите 'help' для списка команд.");
			while (true)
			{
				Console.Write("> ");
				string input = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(input)) continue;
				try
				{
					ICommand command = ParseCommand(input);
					command.Execute();
					if (command is IUndo && !(command is UndoCommand) && !(command is RedoCommand))
					{
						AppInfo.UndoStack.Push(command);
						AppInfo.RedoStack.Clear(); 
					}
				}
				catch (TaskNotFoundException ex)
				{
					Console.WriteLine($"Ошибка задачи: {ex.Message}");
				}
				catch (AuthenticationException ex)
				{
					Console.WriteLine($"Ошибка авторизации: {ex.Message}");
				}
				catch (InvalidCommandException ex)
				{
					Console.WriteLine($"Ошибка команды: {ex.Message}");
				}
				catch (InvalidArgumentException ex)
				{
					Console.WriteLine($"Ошибка аргументов: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Неожиданная ошибка.");
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
				case "add":
					return new AddCommand(args);
				case "view":
					return new ViewCommand { ShowAll = args.Contains("-a") };
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
				case "search":
					return new SearchCommand(args);
				case "undo":
					return new UndoCommand();
				case "redo":
					return new RedoCommand();
				case "profile":
					return new ProfileCommand(input);
				case "help":
					return new CommandHelp();
				case "exit":
					Environment.Exit(0);
					return null!;
				default:
					throw new InvalidCommandException($"Команда '{commandName}' не найдена.");
			}
		}
	}
}