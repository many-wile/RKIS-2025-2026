using System;
namespace TodoList.Commands
{
	internal class UpdateCommand : ICommand
	{
		public int Index { get; set; }
		public string NewText { get; set; }
		private string _oldText;
		public UpdateCommand(int index, string newText)
		{
			Index = index;
			NewText = newText;
		}
		public void Execute()
		{
			var item = AppInfo.Todos.GetItem(Index);
			if (item != null)
			{
				_oldText = item.Text;
				item.UpdateText(NewText);
				Console.WriteLine($"Задача {Index} обновлена.");
			}
			else
			{
				Console.WriteLine("Задача с таким номером не найдена.");
			}
		}
		public void Unexecute()
		{
			var item = AppInfo.Todos.GetItem(Index);
			if (item != null && _oldText != null)
			{
				item.UpdateText(_oldText);
			}
		}
	}
}