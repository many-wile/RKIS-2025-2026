using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
{
    internal class TodoList
    {
        private TodoItem[] items;
        private int count;
        public TodoList()
        {
            items = new TodoItem[2];
            count = 0;
        }
        public void Add(TodoItem item)
        {
            if (count >= items.Length)
                ExpandArray();

            items[count] = item;
            count++;
        }

        public void Delete(int index)
        {
            if (index < 1 || index > count)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }

            int i = index - 1;
            for (int j = i; j < count - 1; j++)
                items[j] = items[j + 1];

            items[count - 1] = null;
            count--;
            Console.WriteLine($"Задача {index} удалена.");
        }
        public TodoItem GetItem(int index)
        {
            if (index < 1 || index > count)
                return null;
            return items[index - 1];
        }

        public void View(bool showIndex, bool showDone, bool showDate)
        {
            if (count == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }
            string header = "";
            if (showIndex) header += "№  ";
            header += "Задача".PadRight(30);
            if (showDone) header += " | Статус".PadRight(10);
            if (showDate) header += " | Дата обновления".PadRight(20);
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
            for (int i = 0; i < count; i++)
            {
                string line = "";

                if (showIndex)
                    line += (i + 1).ToString().PadRight(3);

                line += items[i].GetText().Replace('\n', ' ');
                if (line.Length > 30) line = line.Substring(0, 27) + "...";
                line = line.PadRight(30);

                if (showDone)
                {
                    string status = items[i].GetIsDone() ? "Сделано" : "Не сделано";
                    line += " | " + status.PadRight(10);
                }

                if (showDate)
                {
                    line += " | " + items[i].GetLastUpdate().ToString("dd.MM.yyyy HH:mm");
                }

                Console.WriteLine(line);
            }

            Console.WriteLine(new string('-', header.Length));
        }
        private void ExpandArray()
        {
            int newSize = items.Length * 2;
            TodoItem[] newArray = new TodoItem[newSize];

            for (int i = 0; i < items.Length; i++)
                newArray[i] = items[i];

            items = newArray;
        }
    }
}
