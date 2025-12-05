using System.Collections.Generic;
using System.Linq;
namespace TodoList
{
	public static class ProfileManager
	{
		public static List<Profile> AllProfiles { get; private set; } = new List<Profile>();
		public static void LoadProfiles(string filePath)
		{
			AllProfiles = FileManager.LoadProfiles(filePath);
		}
		public static void SaveProfiles(string filePath)
		{
			FileManager.SaveProfiles(AllProfiles, filePath);
		}
		public static Profile FindByLogin(string login)
		{
			return AllProfiles.FirstOrDefault(p => p.Login.Equals(login, System.StringComparison.OrdinalIgnoreCase));
		}
		public static void AddProfile(Profile profile)
		{
			if (FindByLogin(profile.Login) == null)
			{
				AllProfiles.Add(profile);
			}
		}
	}
}