using System;
namespace TodoList.Commands
{
	internal class StatusCommand : ICommand
	{
		private TodoList _todoList;
		private int _index;
		private TodoStatus _newStatus;
		public StatusCommand(TodoList todoList, int index, TodoStatus newStatus)
		{
			_todoList = todoList;
			_index = index;
			_newStatus = newStatus;
		}
		public void Execute()
		{
			_todoList.SetStatus(_index, _newStatus);
			Console.WriteLine($"Статус задачи {_index} изменен на '{_newStatus}'.");
		}
	}
}