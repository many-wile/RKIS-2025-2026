using System;
namespace TodoList.Commands
{
	internal class StatusCommand : ICommand
	{
		private int _index;
		private TodoStatus _newStatus;
		private TodoStatus _oldStatus;
		public StatusCommand(int index, TodoStatus newStatus)
		{
			_index = index;
			_newStatus = newStatus;
		}
		public void Execute()
		{
			var item = AppInfo.Todos.GetItem(_index);
			if (item != null)
			{
				_oldStatus = item.Status;
				AppInfo.Todos.SetStatus(_index, _newStatus);
				Console.WriteLine($"Статус задачи {_index} изменен на '{_newStatus}'.");
			}
			else
			{
				Console.WriteLine("Задача не найдена.");
			}
		}
		public void Undo()
		{
			var item = AppInfo.Todos.GetItem(_index);
			if (item != null)
			{
				AppInfo.Todos.SetStatus(_index, _oldStatus);
			}
		}
	}
}