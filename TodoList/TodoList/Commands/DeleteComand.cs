namespace TodoList.Commands
{
	internal class DeleteCommand : ICommand
	{
		private int index;
		public DeleteCommand(int index)
		{
			this.index = index;
		}
		public void Execute()
		{
			AppInfo.Todos.Delete(index);
		}
	}
}