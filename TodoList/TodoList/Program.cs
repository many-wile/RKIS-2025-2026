using System;
using System.Globalization;

namespace TodoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Нестеренко и Горелов");
            Console.WriteLine("Введите ваше имя и фамилию:");
            string fullName = Console.ReadLine();
            Console.WriteLine("Введите дату рождения (ДД.MM.ГГГГ):");
            string dateStr = Console.ReadLine();
            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                Console.WriteLine("Ошибка: неверный формат даты!");
                return;
            }
            Profile profile = new Profile(fullName, birthDate);
            TodoList list = new TodoList();
            Console.WriteLine("Введите команду (help — для списка команд):");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                    continue;
                if (input == "exit")
                {
                    Console.WriteLine("Программа завершена.");
                    break;
                }
                if (input == "help")
                {
                    ShowHelp();
                    continue;
                }
                ICommand command = ParseCommand(input, list, profile);

                if (command != null)
                    command.Execute();
                else
                    Console.WriteLine("Неизвестная команда. Введите 'help' для списка команд.");
            }
        }
        static ICommand ParseCommand(string input, TodoList list, Profile profile)
        {
            if (input.StartsWith("add -m") || input.StartsWith("add --multiline"))
            {
                Console.WriteLine("Введите задачу построчно. Для завершения введите !end:");
                string text = "";
                while (true)
                {
                    Console.Write("> ");
                    string line = Console.ReadLine();
                    if (line == "!end" || string.IsNullOrWhiteSpace(line))
                        break;
                    text += (text.Length > 0 ? "\n" : "") + line;
                }
                return new AddCommand(list, text);
            }
            else if (input.StartsWith("add "))
            {
                string text = input.Substring(4).Trim();
                return new AddCommand(list, text);
            }
            else if (input.StartsWith("done "))
            {
                if (int.TryParse(input.Substring(5), out int index))
                    return new DoneCommand(list, index);
            }
            else if (input.StartsWith("update "))
            {
                string[] parts = input.Split(' ', 3);
                if (parts.Length == 3 && int.TryParse(parts[1], out int idx))
                    return new UpdateCommand(list, idx, parts[2]);
            }
            else if (input.StartsWith("view"))
            {
                ViewCommand command = new ViewCommand(list);
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
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
            else if (input.StartsWith("profile"))
            {
                return new ProfileCommand(profile);
            }
            return null;
        }
        static void ShowHelp()
        {
            Console.WriteLine(@"
                Доступные команды:
                 add <текст>          - добавить задачу
                 add -m / --multiline - многострочный ввод (!end - завершить)
                 done <номер>         - отметить задачу выполненной
                 update <номер> <текст> - изменить текст задачи
                 view [флаги]         - показать задачи
                 profile              - показать профиль пользователя
                 exit                 - выход

                Флаги для view:
                 -i, --index          - показывать индекс задачи
                 -s, --status         - показывать статус
                 -d, --update-date    - показывать дату изменения
                 -a, --all            - показывать всё
                ");
        }
    }
}
