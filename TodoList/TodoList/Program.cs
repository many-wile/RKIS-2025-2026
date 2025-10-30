using System;
using System.Globalization;

namespace TodoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Нестеренко и Горелов");
            Console.WriteLine("Введите ваше полное имя:");
            string fullName = Console.ReadLine();
            Console.WriteLine("Введите дату рождения (ДД.MM.ГГГГ):");
            string dateStr = Console.ReadLine();
            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                Console.WriteLine("Ошибка: неверный формат даты!");
                return;
            }
            Profile profile = new Profile(fullName, birthDate);
            TodoList todoList = new TodoList();
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
                ICommand command = CommandParser.Parse(input, todoList, profile);
                if (command != null)
                {
                    command.Execute();
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите 'help' для списка команд.");
                }
            }
        }
        static void ShowHelp()
        {
            Console.WriteLine(@"
            Доступные команды:
             add <текст>          - добавить задачу
             add -m / --multiline - многострочный ввод (!end - завершить)
             done <номер>         - отметить задачу выполненной
             update <номер> <текст> - изменить текст задачи
             delete <номер>       - удалить задачу
             view [флаги]         - показать задачи
             profile              - показать профиль пользователя
             help                 - показать команды
             exit                 - выйти

            Флаги для view:
             -i, --index          - показывать индекс задачи
             -s, --status         - показывать статус задачи
             -d, --update-date    - показывать дату изменения
             -a, --all            - показывать всё
            ");
        }
    }
}
