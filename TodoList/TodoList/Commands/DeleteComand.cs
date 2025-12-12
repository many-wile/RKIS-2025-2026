using System;
namespace TodoList.Commands
{
	public class DeleteCommand : ICommand
	{
		private int _index;
		private TodoItem? _deletedItem;
		public DeleteCommand(int index)
		{
			_index = index;
		}
		public void Execute()
		{
			_deletedItem = AppInfo.CurrentUserTodos.GetItem(_index);
			if (_deletedItem != null)
			{
				AppInfo.CurrentUserTodos.Delete(_index);
				Console.WriteLine($"Задача {_index} удалена.");
			}
			else
			{
				Console.WriteLine("Не удалось удалить задачу. Неверный индекс.");
			}
		}
		public void Unexecute()
		{
			if (_deletedItem != null)
			{
				AppInfo.CurrentUserTodos.Insert(_index, _deletedItem);
			}
		}
	}
}