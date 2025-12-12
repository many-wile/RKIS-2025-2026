using System;
namespace TodoList.Commands
{
	internal class ProfileCommand : ICommand
	{
		private bool _isLogout;
		public ProfileCommand(string inputString)
		{
			_isLogout = false;
			if (!string.IsNullOrWhiteSpace(inputString))
			{
				string[] parts = inputString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length > 1 && (parts[1] == "-o" || parts[1] == "--out"))
				{
					_isLogout = true;
				}
			}
		}
		public void Execute()
		{
			if (_isLogout)
			{
				if (AppInfo.CurrentProfile != null)
				{
					Console.WriteLine($"Пользователь {AppInfo.CurrentProfile.Login} вышел из системы.");
					AppInfo.CurrentProfileId = null;
					AppInfo.UndoStack.Clear();
					AppInfo.RedoStack.Clear();
				}
			}
			else
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
		}
		public void Unexecute()
		{

		}
	}
}