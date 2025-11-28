using System;
namespace TodoList.Commands
{
	internal class AddCommand : ICommand
	{
		public string TaskText { get; set; }
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
			AppInfo.Todos.Add(new TodoItem(TaskText));
			Console.WriteLine("Задача добавлена!");
		}
	}
}