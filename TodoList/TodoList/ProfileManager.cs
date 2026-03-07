using System;
using System.Collections.Generic;
using System.Linq;
namespace TodoList
{
	public static class ProfileManager
	{
		public static List<Profile> AllProfiles { get; set; } = new List<Profile>();

		public static Profile FindByLogin(string login)
		{
			return AllProfiles.FirstOrDefault(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
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