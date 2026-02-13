namespace TodoList.Commands
{
	public interface ICommand
	{
		void Execute();
	}
	public interface IUndo
	{
		void Unexecute();
	}
}