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
			if (inputString == "help")
			{
				ShowHelp();
				return null;
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
			if (inputString.StartsWith("add "))
			{
				string text = inputString.Substring(4).Trim();
				return new AddCommand(text);
			}
			if (inputString.StartsWith("status "))
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
			if (inputString.StartsWith("update "))
			{
				string[] parts = inputString.Split(' ', 3);
				if (parts.Length == 3 && int.TryParse(parts[1], out int idx))
					return new UpdateCommand(idx, parts[2]);
			}
			if (inputString.StartsWith("delete "))
			{
				if (int.TryParse(inputString.Substring(7), out int idx))
					return new DeleteCommand(idx);
			}
			if (inputString.StartsWith("view"))
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
			if (inputString == "profile")
			{
				return new ProfileCommand();
			}
			return null;
		}
		private static void ShowHelp()
		{
			Console.WriteLine(@"
Доступные команды:
add <текст>          - добавить задачу
add -m / --multiline - многострочный ввод (!end - завершить)
status <номер> <статус> - изменить статус задачи
update <номер> <текст> - изменить текст задачи
delete <номер>       - удалить задачу
view [флаги]         - показать задачи
profile              - показать профиль пользователя
help                 - показать список команд
exit                 - выход

Доступные статусы для команды status:
NotStarted, InProgress, Completed, Postponed, Failed

Флаги для view:
-i, --index          - показывать индекс задачи
-s, --status         - показывать статус
-d, --update-date    - показывать дату изменения
-a, --all            - показывать всё
");
		}
	}
}