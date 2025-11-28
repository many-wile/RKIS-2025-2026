using System;
namespace TodoList.Commands
{
	internal class ProfileCommand : ICommand
	{
		public ProfileCommand()
		{
		}
		public void Execute()
		{
			if (AppInfo.CurrentProfile != null)
			{
				Console.WriteLine(AppInfo.CurrentProfile.GetInfo());
			}
			else
			{
				Console.WriteLine("Профиль не загружен.");
			}
		}
		public void Unexecute()
		{

		}
	}
}