using System;
using System.Collections.Generic;
using System.Linq;
using TodoList.Commands;
namespace TodoList
{
	public static class AppInfo
	{
		public static List<Profile> AllProfiles { get; set; } = new List<Profile>();
		public static Guid? CurrentProfileId { get; set; }
		public static TodoList Todos { get; set; }
		public static Profile CurrentProfile
		{
			get
			{
				if (CurrentProfileId == null)
				{
					return null;
				}
				return AllProfiles.FirstOrDefault(p => p.Id == CurrentProfileId);
			}
		}
		public static Stack<ICommand> UndoStack { get; } = new Stack<ICommand>();
		public static Stack<ICommand> RedoStack { get; } = new Stack<ICommand>();
	}
}