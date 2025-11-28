using System;
namespace TodoList.Commands
{
	internal class UpdateCommand : ICommand
	{
		public int Index { get; set; }
		public string NewText { get; set; }
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
				item.UpdateText(NewText);
				Console.WriteLine($"Задача {Index} обновлена.");
			}
			else
			{
				Console.WriteLine("Задача с таким номером не найдена.");
			}
		}
	}
}