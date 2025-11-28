using System;
namespace TodoList.Commands
{
	internal class DeleteCommand : ICommand
	{
		private int _index;
		private TodoItem _deletedItem;
		public DeleteCommand(int index)
		{
			_index = index;
		}
		public void Execute()
		{
			_deletedItem = AppInfo.Todos.GetItem(_index);
			if (_deletedItem != null)
			{
				AppInfo.Todos.Delete(_index);
				Console.WriteLine($"Задача {_index} удалена.");
			}
			else
			{
				Console.WriteLine("Не удалось удалить задачу. Неверный индекс.");
			}
		}
		public void Undo()
		{
			if (_deletedItem != null)
			{
				AppInfo.Todos.Insert(_index, _deletedItem);
			}
		}
	}
}