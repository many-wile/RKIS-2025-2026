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
		public static Dictionary<Guid, TodoList> AllTodos { get; private set; } = new Dictionary<Guid, TodoList>();
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
		public static TodoList CurrentUserTodos
		{
			get
			{
				if (CurrentProfileId == null)
				{
					return null;
				}
				if (!AllTodos.ContainsKey(CurrentProfileId.Value))
				{
					AllTodos[CurrentProfileId.Value] = new TodoList();
				}
				return AllTodos[CurrentProfileId.Value];
			}
		}
		public static Stack<ICommand> UndoStack { get; } = new Stack<ICommand>();
		public static Stack<ICommand> RedoStack { get; } = new Stack<ICommand>();
	}
}