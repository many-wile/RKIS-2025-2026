using System;
namespace TodoList.Commands
{
	internal class AddCommand : ICommand
	{
		public string TaskText { get; set; }
		private TodoItem _addedItem;
		public AddCommand(string taskText)
		{
			TaskText = taskText;
		}
		public void Execute()
		{
			if (string.IsNullOrWhiteSpace(TaskText))
			{
				Console.WriteLine("Текст задачи не может быть пустым.");
				return;
			}
			_addedItem = new TodoItem(TaskText);
			AppInfo.Todos.Add(_addedItem);
			Console.WriteLine("Задача добавлена!");
		}
		public void Undo()
		{
			if (AppInfo.Todos.Count > 0)
			{
				AppInfo.Todos.Delete(AppInfo.Todos.Count);
			}
		}
	}
}