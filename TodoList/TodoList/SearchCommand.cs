using System;
namespace TodoList.Commands
{
	public class SearchCommand : ICommand
	{
		private string _query;
		public SearchCommand(string query)
		{
			_query = query;
		}
		public void Execute()
		{
			if (string.IsNullOrWhiteSpace(_query))
			{
				Console.WriteLine("Ошибка: Введите текст для поиска.");
				return;
			}
			var todos = AppInfo.CurrentUserTodos;
			if (todos.Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
				return;
			}
			Console.WriteLine($"Result search \"{_query}\":");
			bool found = false;
			for (int i = 0; i < todos.Count; i++)
			{
				var item = todos[i];
				if (item.Text.IndexOf(_query, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					Console.WriteLine($"[{i + 1}] {item.GetShortInfo()}");
					found = true;
				}
			}

			if (!found)
			{
				Console.WriteLine("Задачи не найдены.");
			}
		}

		public void Unexecute()
		{
		}
	}
}