using System;
using System.Globalization;

namespace TodoList
{
    internal class Program
    {
        static string fullName;
        static int age;
        static string[] todos = new string[2];
        static bool[] statuses = new bool[2];
        static DateTime[] dates = new DateTime[2];
        static int todosCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Нестеренко и Горелов");
            Console.WriteLine("Будьте любезны ввести свои фамилию и имя!");
            fullName = Console.ReadLine();

            Console.WriteLine("Теперь введите дату рождения в формате ДД.ММ.ГГГГ");
            string dat = Console.ReadLine();
            if (!DateTime.TryParseExact(dat, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                Console.WriteLine("Неверный формат возраста!");
                return;
            }

            age = DateTime.Today.Year - birthDate.Year;
            Console.WriteLine($"Добавлен пользователь {fullName}, возраст - {age} лет");
            Console.WriteLine("Введите команду:");

            while (true)
            {
                var input = Console.ReadLine();

                switch (input)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "profile":
                        ShowProfile();
                        break;
                    case string s when s.StartsWith("view"):
                        {
                        bool showIndex = s.Contains("--index") || s.Contains("-i");
                        bool showStatus = s.Contains("--status") || s.Contains("-s");
                        bool showDate = s.Contains("--update-date") || s.Contains("-d");
                        bool showAll = s.Contains("--all") || s.Contains("-a");
                        ViewTasks(showIndex, showStatus, showDate, showAll);
                        break;
                        }
                    case "exit":
                        ExitProgram();
                        break;
                    default:
                        if (input.StartsWith("add "))
                        { 
                            var arg = input.Substring(4).Trim();
                            if (arg == "--multiline" || arg == "-m")
                                AddTasksMultiline();
                            else
                                AddTask(arg);

                        }
                        else if (input.StartsWith("done "))
                            CompleteTask(input.Substring(5));
                        else if (input.StartsWith("delete "))
                            DeleteTask(input.Substring(7));
                        else if (input.StartsWith("update "))
                            UpdateTask(input.Substring(7));
                        else if (input.StartsWith("read "))
                        {
                            ReadTask(input.Substring(5).Trim());
                        }
                        else
                            Console.WriteLine("Неизвестная команда. Введите 'help' для списка доступных команд.");
                        break;
                }
            }
        }

        static void AddTasksMultiline()
        {
            Console.WriteLine("Введите задачи прострочно. Пустая строка завершит ввод");
            string fullTask = "";

            while(true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == "!end")
                break;

                if (!string.IsNullOrEmpty(fullTask))
                    fullTask += "\n";
                fullTask += line;
            }

            if (!string.IsNullOrWhiteSpace(fullTask))
            {
                AddTask(fullTask);
            }
            else
            {
                Console.WriteLine("Задача пуста, не добавлена.");
            }
        }
        static void CompleteTask(string indexStr)
        {
            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }
            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }
            int i = index - 1;
            statuses[i] = true;
            dates[i] = DateTime.Now;
            Console.WriteLine($"Задача {index} выполнена.");
        }

        static void DeleteTask(string indexStr)
        {
            if (!int.TryParse(indexStr, out int index)) 
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }
            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }

            int i = index - 1;
            for (int j = i; j < todosCount - 1; j++)
            {
                todos[j] = todos[j + 1];
                statuses[j] = statuses[j + 1];
                dates[j] = dates[j + 1];
            }
            todosCount--;
            Console.WriteLine($"Задача {index} удалена.");
        }

        static void UpdateTask(string input)
        {
            int firstSpace = input.IndexOf(' ');
            if (firstSpace == -1)
            {
                Console.WriteLine("Неверный формат команды. Используйте: update <номер> \"новый текст\"");
                return;
            }

            string indexStr = input.Substring(0, firstSpace);
            string newText = input.Substring(firstSpace + 1).Trim();

            if (newText.StartsWith("\"") && newText.EndsWith("\""))
                newText = newText[1..^1];

            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }
            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }

            int i = index - 1;
            todos[i] = newText;
            dates[i] = DateTime.Now;
            Console.WriteLine($"Задача {index} обновлена: {newText}");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Доступные команды: ");
            Console.WriteLine("profile - выводит данные о пользователе");
            Console.WriteLine("add - добавляет новую задачу");
            Console.WriteLine("view - выводит все задачи из массива");
            Console.WriteLine("read <idx> - показывает полную информацию о задаче");
            Console.WriteLine("done <idx> - отмечает задачу выполненной");
            Console.WriteLine("delete <idx> - удаляет задачу по индексу");
            Console.WriteLine("update <idx> \"new_text\" - обновляет текст задачи");
            Console.WriteLine("exit - завершает цикл и останавливает выполнение программы");
            Console.WriteLine("Флаги:");
            Console.WriteLine("  --multiline или -m - многострочный ввод для add");
            Console.WriteLine("  -i - показывать только невыполненные задачи для view");
            Console.WriteLine("  -s - показывать статистику для view");
            Console.WriteLine("Флаги для view:");
            Console.WriteLine("  --index или -i - показывать индекс задачи");
            Console.WriteLine("  --status или -s - показывать статус задачи");
            Console.WriteLine("  --update-date или -d - показывать дату изменения");
            Console.WriteLine("  --all или -a - показывать все данные");
            Console.WriteLine("  --incomplete или -I - показывать только невыполненные");
            Console.WriteLine("  --statistics или -S - показывать статистику");
            Console.WriteLine("Примеры: view -isd, view --all, view -i --status");
        }

        static void ShowProfile()
        {
            Console.WriteLine($"{fullName}, возраст: {age} лет");
        }

        static void AddTask(string taskText)
        {
            if (todosCount >= todos.Length)
                ExpandArray();
            
            todos[todosCount] = taskText;
            statuses[todosCount] = false;
            dates[todosCount] = DateTime.Now;
            todosCount++;

            Console.WriteLine($"Задача добавлена: {taskText}");
        }

        static void ExpandArray()
        {
            int newSize = todos.Length * 2;
            string[] newTodos = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];

            for (int i = 0; i < todos.Length; i++)
            {
                newTodos[i] = todos[i];
                newStatuses[i] = statuses[i];
                newDates[i] = dates[i];
            }

            todos = newTodos;
            statuses = newStatuses;
            dates = newDates;
        }

        static void ViewTasks(bool showIndex = false, bool showStatus = false, bool showDate = false, bool showAll = false)
        {
            if (todosCount == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }

            int indexWidth = 5;
            int textWidth = 30;   
            int statusWidth = 12;
            int dateWidth = 20;
            string header = "";
            if (showIndex || showAll) header += $"{"№".PadRight(indexWidth)} ";
            header += $"{"Задача".PadRight(textWidth)} ";
            if (showStatus || showAll) header += $"{"Статус".PadRight(statusWidth)} ";
            if (showDate || showAll) header += $"{"Дата обновления".PadRight(dateWidth)}";
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            for (int i = 0; i < todosCount; i++)
            {
                string line = "";
                if (showIndex || showAll)
                    line += $"{(i + 1).ToString().PadRight(indexWidth)} ";
                string taskText = todos[i];
                if (taskText.Length > textWidth)
                    taskText = taskText.Substring(0, textWidth - 3) + "...";
                line += taskText.PadRight(textWidth) + " ";

                if (showStatus || showAll)
                    line += (statuses[i] ? "Сделано" : "Не сделано").PadRight(statusWidth) + " ";

                if (showDate || showAll)
                    line += dates[i].ToString("dd.MM.yyyy HH:mm").PadRight(dateWidth);

                Console.WriteLine(line);
            }
        }
        static void ReadTask(string indexStr)
        {
            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }

            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }

            int i = index - 1;

            Console.WriteLine("Полный текст задачи:");
            Console.WriteLine(todos[i]);
            Console.WriteLine($"Статус: {(statuses[i] ? "Выполнена" : "Не выполнена")}");
            Console.WriteLine($"Дата последнего изменения: {dates[i]:dd.MM.yyyy HH:mm}");
        }

        static void ExitProgram()
        {
            Console.WriteLine("Завершение программы...");
            Environment.Exit(0);
        }

    }
}



