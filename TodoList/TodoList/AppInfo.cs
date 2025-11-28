using System;
using System.Collections.Generic;
using TodoList.Commands;
namespace TodoList
{
	static class AppInfo
	{
		public static TodoList Todos { get; set; }
		public static Profile CurrentProfile { get; set; }
		public static Stack<ICommand> UndoStack { get; } = new Stack<ICommand>();
		public static Stack<ICommand> RedoStack { get; } = new Stack<ICommand>();
	}
}