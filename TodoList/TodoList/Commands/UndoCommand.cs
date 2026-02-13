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
				if (lastCommand is IUndo undoableCommand)
				{
					undoableCommand.Unexecute();
					AppInfo.RedoStack.Push(lastCommand);
					Console.WriteLine("Действие отменено.");
				}
				else
				{
					Console.WriteLine("Эту команду нельзя отменить.");
				}
			}
			else
			{
				Console.WriteLine("Нечего отменять.");
			}
		}
	}
}