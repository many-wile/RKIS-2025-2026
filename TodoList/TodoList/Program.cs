using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Threading.Tasks;
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
            Console.WriteLine("будьте любезны ввести свои фамилию и имя!");
            fullName = Console.ReadLine();
            Console.WriteLine("Теперь введите дату рождения  в формате ДД.ММ.ГГГГ");
            DateTime birthDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.Today;
            age = currentDate.Year - birthDate.Year;
            Console.WriteLine("Добавлен пользователь " + fullName + ", " + "возраст - " + age + " лет");
            Console.WriteLine("Введите команду: ");
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

                    case "view":
                        ViewTasks();
                        break;

                    case "exit":
                        ExitProgram();
                        break;

                    default:
                        if (input.StartsWith("add "))
                        {
                            AddTask(input.Substring(4));
                        }
                        else
                        { 
                        Console.WriteLine("Неизвестная команда. Введите 'help' для списка доступных команд.");
                        }
                            break;
                }
            }
        }
        static void ShowHelp()
        {
            Console.WriteLine("profile - выводит данные пользователя");
            Console.WriteLine("add - добавляет новую задачу");
            Console.WriteLine("view - выводит все задачи из массива");
            Console.WriteLine("exit - завершает цикл и останавливает выполнение программы.");
        }
        static void ShowProfile()
        {
            Console.WriteLine($"{fullName}, возраст: {age} лет");
        }
        static void AddTask(string taskText)
        {
            if (todosCount >= todos.Length)
            {
                ExpandArray();
            }
            todos[todosCount] = taskText;
            statuses[todosCount] = false;
            dates[todosCount] = DateTime.Now:
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
        static void ViewTasks()
            {
                Console.WriteLine("Ваши задачи:");
                if (todosCount == 0)
                {
                    Console.WriteLine("Нет задач.");
                    return;
                }

                for (int i = 0; i < todosCount; i++)
                {
                    Console.WriteLine($"{i + 1}. {todos[i]}, (дата: {dates[i]})");
                }
            }
        static void ExitProgram()
            {
                Console.WriteLine("Завершение программы...");
                Environment.Exit(0);
            }

    }

}


