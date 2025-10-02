using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Threading.Tasks;
namespace grib
{
    internal class Program
    {
        static void Main(string[] args)

        {
            Console.WriteLine("Работу выполнили Нестеренко и Горелов");
            Console.WriteLine("будьте любезны ввести свои фамилию и имя!");
            string fullName = Console.ReadLine();
            Console.WriteLine("Теперь введите дату рождения  в формате ДД.ММ.ГГГГ");
            DateTime birthDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - birthDate.Year;
            Console.WriteLine("Добавлен пользователь " + fullName + ", " + "возраст - " + age + " лет");
            string[] tasks = new string[10];
            int taskCount = 0;
            string[] todos = { "help", "profile", "add", "view", "exit" };
            todos[1] = "profile - выводит данные пользователя";
            todos[2] = "add - добавляет новую задачу";
            todos[3] = "view - выводит все задачи из массива";
            todos[4] = "exit - завершает цикл и останавливает выполнение программы.";
            Console.WriteLine("Введите команду: ");
            while (true)
            {
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.Write("что вы наделали");
                    break;
                }

                switch (input)
                {
                    case "help":
                        Console.WriteLine(todos[1]);
                        Console.WriteLine(todos[2]);
                        Console.WriteLine(todos[3]);
                        Console.WriteLine(todos[4]);
                        break;

                    case "profile":
                        Console.WriteLine(fullName + " " + "Возраст: " + age);
                        break;

                    case "add":
                        Console.WriteLine("введите команду");
                        string task = Console.ReadLine();
                        string[] parts = task.Split(' ', 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Ошибка: используйте формат add текст задачи");
                        }
                        else
                        {
                            tasks[taskCount++] = parts[1];
                            Console.WriteLine("Задача добавлена: " + parts[1]);
                        }
                        break;


                    case "view":
                        Console.WriteLine("Ваши задачи:");
                        for (int i = 0; i < taskCount; i++)
                        {
                            Console.WriteLine($"{i + 1}. {tasks[i]}");
                        }
                        if (taskCount == 0) Console.WriteLine("Нет задач.");
                        break;
                         
                    case "exit":
                        Console.WriteLine("Завершение задачи");
                        return;

                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }
    }

}


