using System;
namespace TodoList.Commands
{
	public class UpdateCommand : ICommand, IUndo
	{
		public int Index { get; set; }
		public string NewText { get; set; }
		private string? _oldText;
		public UpdateCommand(int index, string newText)
		{
			Index = index;
			NewText = newText;
		}
		public void Execute()
		{
			var item = AppInfo.CurrentUserTodos.GetItem(Index);
			if (item != null)
			{
				_oldText = item.Text;
				AppInfo.CurrentUserTodos.Update(Index, NewText);
				Console.WriteLine($"Задача {Index} обновлена.");
			}
			else Console.WriteLine("Задача не найдена.");
		}
		public void Unexecute()
		{
			var item = AppInfo.CurrentUserTodos.GetItem(Index);
			if (item != null && _oldText != null)
				AppInfo.CurrentUserTodos.Update(Index, _oldText);
		}
	}
}