using System;
using TodoList.Commands;
namespace TodoList.Commands
{
	internal class RedoCommand : ICommand
	{
		public void Execute()
		{
			if (AppInfo.RedoStack.Count > 0)
			{
				ICommand commandToRedo = AppInfo.RedoStack.Pop();
				commandToRedo.Execute();
				AppInfo.UndoStack.Push(commandToRedo);

				Console.WriteLine("Действие повторено.");
			}
			else
			{
				Console.WriteLine("Нечего повторять (Redo стек пуст).");
			}
		}
		public void Unexecute()
		{

		}
	}
}