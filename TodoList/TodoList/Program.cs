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
        }

        static void second(string[] args)
        {
            string[] todos;
            while (true)
            {
                if (ShouldStop( help , profile , add , view , exit , )) break;
            }
        }
    }
 } 

