using System;
using System.Collections;
using System.Collections.Generic;

namespace TodoList
{
	internal class TodoList : IEnumerable<TodoItem>
	{
		private List<TodoItem> items;
		public TodoList()
		{
			items = new List<TodoItem>();
		}
		public int Count
		{
			get { return items.Count; }
		}
		public TodoItem this[int index]
		{
			get
			{
				if (index < 0 || index >= items.Count)
				{
					throw new IndexOutOfRangeException("Индекс находится за пределами списка задач.");
				}
				return items[index];
			}
			set
			{
				if (index < 0 || index >= items.Count)
				{
					throw new IndexOutOfRangeException("Индекс находится за пределами списка задач.");
				}
				items[index] = value;
			}
		}
		public void Add(TodoItem item)
		{
			items.Add(item);
		}
		public void Delete(int index)
		{
			int internalIndex = index - 1;

			if (internalIndex < 0 || internalIndex >= items.Count)
			{
				Console.WriteLine("Задачи с таким номером нет.");
				return;
			}
			items.RemoveAt(internalIndex);
			Console.WriteLine($"Задача {index} удалена.");
		}
		public TodoItem GetItem(int index)
		{
			int internalIndex = index - 1;
			if (internalIndex < 0 || internalIndex >= items.Count)
			{
				return null;
			}
			return items[internalIndex];
		}
		public void View(bool showIndex, bool showDone, bool showDate)
		{
			if (items.Count == 0)
			{
				Console.WriteLine("Нет задач.");
				return;
			}
			string header = "";
			if (showIndex) header += "№  ";
			header += "Задача".PadRight(30);
			if (showDone) header += " | Статус".PadRight(12);
			if (showDate) header += " | Дата обновления".PadRight(20);
			Console.WriteLine(header);
			Console.WriteLine(new string('-', header.Length));
			for (int i = 0; i < items.Count; i++)
			{
				string line = "";
				if (showIndex)
					line += (i + 1).ToString().PadRight(3);
				line += items[i].Text.Replace('\n', ' ');
				if (line.Length > 30) line = line.Substring(0, 27) + "...";
				line = line.PadRight(30);

				if (showDone)
				{
					string status = items[i].Status.ToString();
					line += " | " + status.PadRight(12);
				}
				if (showDate)
				{
					line += " | " + items[i].LastUpdate.ToString("dd.MM.yyyy HH:mm");
				}
				Console.WriteLine(line);
			}
			Console.WriteLine(new string('-', header.Length));
		}
		public IEnumerator<TodoItem> GetEnumerator()
		{
			for (int i = 0; i < items.Count; i++)
			{
				yield return items[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}