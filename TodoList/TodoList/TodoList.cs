using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
    {
        internal class TodoList
        {
            private TodoItem[] items = new TodoItem[2];
            private int count = 0;

            public void Add(TodoItem item)
            {
                if (count >= items.Length)
                    ExpandArray();

                items[count] = item;
                count++;
                Console.WriteLine($"Задача добавлена: {item.Text}");
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

            public void View(bool showIndex = false, bool showDone = false, bool showDate = false)
            {
                if (count == 0)
                {
                    Console.WriteLine("Нет задач.");
                    return;
                }

                int indexWidth = 5;
                int textWidth = 30;
                int statusWidth = 12;
                int dateWidth = 20;

                string header = "";
                if (showIndex) header += "№".PadRight(indexWidth) + " ";
                header += "Задача".PadRight(textWidth) + " ";
                if (showDone) header += "Статус".PadRight(statusWidth) + " ";
                if (showDate) header += "Дата обновления".PadRight(dateWidth);

                Console.WriteLine(header);
                Console.WriteLine(new string('-', header.Length));

                for (int i = 0; i < count; i++)
                {
                    string line = "";
                    if (showIndex) line += (i + 1).ToString().PadRight(indexWidth) + "";
                    string taskText = items[i].Text.Split('\n')[0];
                    if (taskText.Length > textWidth)
                    taskText = taskText.Substring(0, textWidth - 3) + "...";
                line += taskText.PadRight(textWidth) + " ";

                    if (showDone) line += (items[i].IsDone ? "Сделано" : "Не сделано").PadRight(statusWidth) + " ";
                    if (showDate) line += items[i].LastUpdate.ToString("dd.MM.yyyy HH:mm").PadRight(dateWidth);

                    Console.WriteLine(line);
                }
            }

            public TodoItem GetItem(int index)
            {
                if (index < 1 || index > count)
                    return null;

                return items[index - 1];
            }

            private void ExpandArray()
            {
                int newSize = items.Length * 2;
                TodoItem[] newItems = new TodoItem[newSize];

                for (int i = 0; i < count; i++)
                    newItems[i] = items[i];

                items = newItems;
            }
        }
    }
