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
            string[] todos = new string[2];
            int todosCount = 0;
            Console.WriteLine("Введите команду: ");
            while (true)
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "help":
                        Console.WriteLine("profile - выводит данные пользователя");
                        Console.WriteLine("add - добавляет новую задачу");
                        Console.WriteLine("view - выводит все задачи из массива");
                        Console.WriteLine("exit - завершает цикл и останавливает выполнение программы.");
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
                            if (todosCount >= todos.Length) ;
                            {
                            string[] newTodos = new string[todos.Length * 2];
                            for (int i = 0; i < task.Length; i++);
                            todos = newTodos;
                            }
                            todos[todosCount++] = parts[1];
                            Console.WriteLine("Задача добавлена: " + parts[1]);
                        }
                        break;


                    case "view":
                        Console.WriteLine("Ваши задачи:");
                        for (int i = 0; i < todosCount; i++)
                        {
                            Console.WriteLine($"{i + 1}. {todos[i]}");
                        }
                        if (todosCount == 0) Console.WriteLine("Нет задач.");
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


