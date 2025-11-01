using System;
using TodoList.Commands;
namespace TodoList
{
    static class CommandParser
    {
        public static ICommand Parse(string inputString, TodoList todoList, Profile profile)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return null;
            inputString = inputString.Trim();
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
                return new AddCommand(todoList, text);
            }
            if (inputString.StartsWith("add "))
            {
                string text = inputString.Substring(4).Trim();
                return new AddCommand(todoList, text);
            }
            if (inputString.StartsWith("done "))
            {
                if (int.TryParse(inputString.Substring(5), out int idx))
                    return new DoneCommand(todoList, idx);
            }
            if (inputString.StartsWith("update "))
            {
                string[] parts = inputString.Split(' ', 3);
                if (parts.Length == 3 && int.TryParse(parts[1], out int idx))
                    return new UpdateCommand(todoList, idx, parts[2]);
            }
            if (inputString.StartsWith("delete "))
            {
                if (int.TryParse(inputString.Substring(7), out int idx))
                    return new DeleteCommand(todoList, idx);
            }
            if (inputString.StartsWith("view"))
            {
                ViewCommand command = new ViewCommand(todoList);
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
                return new ProfileCommand(profile);
            }
            if (inputString == "help")
            {
                return new CommandHelp();
            }
            return null;
        }
    }
}
