using System;
namespace TodoList.Commands
{
	internal class ViewCommand : ICommand
	{
		public bool ShowIndex { get; set; }
		public bool ShowDone { get; set; }
		public bool ShowDate { get; set; }
		public bool ShowAll { get; set; }
		public ViewCommand()
		{
		}
		public void Execute()
		{
			if (ShowAll)
			{
				ShowIndex = ShowDone = ShowDate = true;
			}
			AppInfo.Todos.View(ShowIndex, ShowDone, ShowDate);
		}
		public void Undo()
		{
		}
	}
}