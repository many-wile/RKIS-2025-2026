using System;
namespace TodoList.Commands
{
	public class ViewCommand : ICommand
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
			AppInfo.CurrentUserTodos.View(ShowIndex, ShowDone, ShowDate);
		}
		public void Unexecute()
		{
		}
	}
}