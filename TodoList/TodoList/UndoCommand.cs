using System;
using TodoList.Commands;
namespace TodoList.Commands
{
	internal class UndoCommand : ICommand
	{
		public void Execute()
		{
			if (AppInfo.UndoStack.Count > 0)
			{
				ICommand lastCommand = AppInfo.UndoStack.Pop();
				lastCommand.Unexecute();
				AppInfo.RedoStack.Push(lastCommand);

				Console.WriteLine("Действие отменено.");
			}
			else
			{
				Console.WriteLine("Нечего отменять.");
			}
		}

		public void Unexecute()
		{

		}
	}
}