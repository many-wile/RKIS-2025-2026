namespace TodoList.Commands
{
	internal interface ICommand
	{
		void Execute();
		void Unexecute();
	}
}