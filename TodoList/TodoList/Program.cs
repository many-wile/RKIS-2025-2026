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
                Console.WriteLine("Неверный формат даты!");
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
                if (input.ToLower() == "exit")
                    break;
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
            Console.WriteLine("Программа завершена.");
        }
    }
}
