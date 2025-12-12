using System;
namespace TodoList.Commands
{
	public class AddCommand : ICommand
	{
		public string TaskText { get; set; }
		private TodoItem? _addedItem;
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
			AppInfo.CurrentUserTodos.Add(_addedItem);
			Console.WriteLine("Задача добавлена!");
		}
		public void Unexecute()
		{
			if (_addedItem != null && AppInfo.CurrentUserTodos.Contains(_addedItem))
			{
				AppInfo.CurrentUserTodos.Remove(_addedItem);
			}
		}
	}
}