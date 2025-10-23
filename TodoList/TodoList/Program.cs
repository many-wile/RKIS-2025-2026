using System;
using System.Globalization;

namespace TodoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Нестеренко и Горелов");
            Console.WriteLine("Введите полное имя:");
            string fullName = Console.ReadLine();

            Console.WriteLine("Введите дату рождения (ДД.MM.ГГГГ):");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                Console.WriteLine("Неверный формат даты!");
                return;
            }

            Profile profile = new Profile(fullName, birthDate.Year);
            Console.WriteLine($"Пользователь: {profile.GetInfo()}");
            TodoList todoList = new TodoList();

            Console.WriteLine("Введите команду (help - список команд):");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input == "exit")
                    break;
                else if (input == "help")
                    ShowHelp();
                else if (input == "profile")
                    Console.WriteLine(profile.GetInfo());
                else if (input.StartsWith("add -m") || input.StartsWith("add --multiline"))
                    AddMultiline(todoList);
                else if (input.StartsWith("add "))
                {
                    string taskText = input.Substring(4).Trim();
                    TodoItem task = new TodoItem(taskText);
                    todoList.Add(task);
                    Console.WriteLine("Задача добавлена.");
                }
                else if (input.StartsWith("done "))
                {
                    if (int.TryParse(input.Substring(5), out int idx))
                    {
                        TodoItem item = todoList.GetItem(idx);
                        if (item != null)
                        {
                            item.MarkDone();
                            Console.WriteLine($"Задача {idx} выполнена.");
                        }
                        else
                            Console.WriteLine("Задачи с таким номером нет.");
                    }
                    else
                        Console.WriteLine("Неверный номер задачи.");
                }
                else if (input.StartsWith("update "))
                {
                    string[] parts = input.Split(' ', 3);
                    if (parts.Length >= 3 && int.TryParse(parts[1], out int idx))
                    {
                        TodoItem item = todoList.GetItem(idx);
                        if (item != null)
                        {
                            item.UpdateText(parts[2]);
                            Console.WriteLine($"Задача {idx} обновлена.");
                        }
                        else
                            Console.WriteLine("Задачи с таким номером нет.");
                    }
                    else
                        Console.WriteLine("Используйте: update <номер> \"текст\"");
                }
                else if (input.StartsWith("delete "))
                {
                    if (int.TryParse(input.Substring(7), out int idx))
                        todoList.Delete(idx);
                    else
                        Console.WriteLine("Неверный номер задачи.");
                }
                else if (input.StartsWith("view"))
                    ParseAndView(todoList, input);
                else if (input.StartsWith("read "))
                {
                    if (int.TryParse(input.Substring(5), out int idx))
                    {
                        TodoItem item = todoList.GetItem(idx);
                        if (item != null)
                            Console.WriteLine(item.GetFullInfo());
                        else
                            Console.WriteLine("Задачи с таким номером нет.");
                    }
                    else
                        Console.WriteLine("Неверный номер задачи.");
                }
                else
                    Console.WriteLine("Неизвестная команда. help - список команд.");
            }

            Console.WriteLine("Программа завершена.");
        }

        static void ShowHelp()
        {
            Console.WriteLine(@"
                    Доступные команды:
                     add <текст>          - добавить задачу
                     add -m / --multiline - многострочный ввод (завершить '!end' или пустой строкой)
                     done <номер>         - отметить задачу выполненной
                     update <номер> <текст> - обновить текст задачи
                     delete <номер>       - удалить задачу
                     view [флаги]         - показать задачи (см. флаги)
                     read <номер>         - показать полное описание задачи
                     profile              - информация о пользователе
                     help                 - показать команды
                     exit                 - выйти

                    Флаги для view:
                     -i, --index          - показывать индекс задачи
                     -s, --status         - показывать статус задачи
                     -d, --update-date    - показывать дату последнего изменения
                     -a, --all            - показывать всё (индекс, статус, дату)
                    ");
         }

        static void AddMultiline(TodoList tasks)
        {
            Console.WriteLine("Введите текст задачи построчно. '!end' или пустая строка для завершения:");
            string fullText = "";
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line) || line.Trim() == "!end")
                    break;

                if (!string.IsNullOrEmpty(fullText)) fullText += "\n";
                fullText += line;
            }

            if (!string.IsNullOrWhiteSpace(fullText))
            {
                TodoItem task = new TodoItem(fullText);
                tasks.Add(task);
                Console.WriteLine("Многострочная задача добавлена.");
            }
            else
                Console.WriteLine("Задача пуста, не добавлена.");
        }

        static void ParseAndView(TodoList tasks, string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            bool showIndex = false;
            bool showStatus = false;
            bool showDate = false;
            bool showAll = false;

            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.StartsWith("--"))
                {
                    if (part == "--index") showIndex = true;
                    else if (part == "--status") showStatus = true;
                    else if (part == "--update-date") showDate = true;
                    else if (part == "--all") showAll = true;
                }
                else if (part.StartsWith("-") && part.Length > 1)
                {
                    for (int j = 1; j < part.Length; j++)
                    {
                        switch (part[j])
                        {
                            case 'i': showIndex = true; break;
                            case 's': showStatus = true; break;
                            case 'd': showDate = true; break;
                            case 'a': showAll = true; break;
                        }
                    }
                }
            }

            if (showAll)
                showIndex = showStatus = showDate = true;

            tasks.View(showIndex, showStatus, showDate);
        }
    }
}
