using System;
namespace TodoList.Commands
{
	internal class StatusCommand : ICommand
	{
		private int _index;
		private TodoStatus _newStatus;
		public StatusCommand(int index, TodoStatus newStatus)
		{
			_index = index;
			_newStatus = newStatus;
		}
		public void Execute()
		{
			AppInfo.Todos.SetStatus(_index, _newStatus);
			Console.WriteLine($"Статус задачи {_index} изменен на '{_newStatus}'.");
		}
	}
}