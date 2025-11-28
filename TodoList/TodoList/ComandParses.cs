using System;
using TodoList.Commands;
namespace TodoList
{
	static class CommandParser
	{
		public static ICommand Parse(string inputString)
		{
			if (string.IsNullOrWhiteSpace(inputString))
				return null;
			inputString = inputString.Trim();
			string commandName = inputString.Split(' ')[0].ToLower();
			if (commandName == "help")
			{
				return new CommandHelp();
			}
			if (inputString.StartsWith("add -m") || inputString.StartsWith("add --multiline"))
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
			if (commandName == "add")
			{
				if (inputString.Length > 4)
				{
					string text = inputString.Substring(4).Trim();
					return new AddCommand(text);
				}
			}
			if (commandName == "status")
			{
				string[] parts = inputString.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 3 && int.TryParse(parts[1], out int idx))
				{
					if (Enum.TryParse<TodoStatus>(parts[2], true, out TodoStatus newStatus))
					{
						return new StatusCommand(idx, newStatus);
					}
					else
					{
						Console.WriteLine($"Неизвестный статус: '{parts[2]}'.");
						Console.WriteLine("Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed.");
						return null;
					}
				}
			}
			if (commandName == "update")
			{
				string[] parts = inputString.Split(' ', 3);
				if (parts.Length == 3 && int.TryParse(parts[1], out int idx))
					return new UpdateCommand(idx, parts[2]);
			}
			if (commandName == "delete")
			{
				if (int.TryParse(inputString.Substring(7), out int idx))
					return new DeleteCommand(idx);
			}
			if (commandName == "view")
			{
				ViewCommand command = new ViewCommand();
				string[] parts = inputString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				for (int i = 1; i < parts.Length; i++)
				{
					switch (parts[i])
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
			if (commandName == "profile")
			{
				return new ProfileCommand();
			}
			return null;
		}
	}
}