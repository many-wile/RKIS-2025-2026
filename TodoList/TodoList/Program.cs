using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime;
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
                        Console.WriteLine(fullName + age);
                        break;

                    case "add":
                        string task = Console.ReadLine();
                        break;

                }
            }
        }
    }

}


