using System;
using System.IO;
using System.Collections.Generic;
namespace TodoList
{
	static class FileManager
	{
		public static void SaveTodoList(TodoItem item)
		{
			if (AppInfo.CurrentProfileId != null)
			{
				SaveTodos(AppInfo.CurrentUserTodos, AppInfo.CurrentUserTodosPath);
			}
		}
		public static void EnsureDataDirectory(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
		}
		public static void SaveProfiles(List<Profile> profiles, string filePath)
		{
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				using (StreamWriter writer = new StreamWriter(fs))
				{
					writer.WriteLine("Id;Login;Password;FirstName;LastName;BirthYear");
					foreach (var profile in profiles)
					{
						writer.WriteLine(profile.ToCsvString());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при сохранении профилей в '{filePath}': {ex.Message}");
			}
		}
		public static List<Profile> LoadProfiles(string filePath)
		{
			var profiles = new List<Profile>();
			if (!File.Exists(filePath))
			{
				return profiles;
			}
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs))
				{
					string header = reader.ReadLine();
					if (header == null) return profiles;

					string line;
					while ((line = reader.ReadLine()) != null)
					{
						string[] parts = line.Split(';');
						if (parts.Length == 6)
						{
							if (Guid.TryParse(parts[0], out Guid id) && int.TryParse(parts[5], out int birthYear))
							{
								string login = parts[1];
								string password = parts[2];
								string firstName = parts[3];
								string lastName = parts[4];
								profiles.Add(new Profile(id, login, password, firstName, lastName, birthYear));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при загрузке профилей из '{filePath}': {ex.Message}");
			}
			return profiles;
		}
		public static void SaveTodos(TodoList todos, string filePath)
		{
			if (todos == null) return;
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				using (StreamWriter writer = new StreamWriter(fs))
				{
					writer.WriteLine("Index;Text;Status;LastUpdate");
					for (int i = 0; i < todos.Count; i++)
					{
						TodoItem item = todos[i];
						if (item != null)
						{
							string processedText = item.Text.Replace("\n", "\\n").Replace("\"", "\"\"");
							if (processedText.Contains(";"))
							{
								processedText = $"\"{processedText}\"";
							}
							writer.WriteLine($"{i};{processedText};{item.Status.ToString()};{item.LastUpdate:o}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при сохранении задач в '{filePath}': {ex.Message}");
			}
		}
		public static TodoList LoadTodos(string filePath)
		{
			TodoList todos = new TodoList();
			if (!File.Exists(filePath))
			{
				return todos;
			}
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs))
				{
					string header = reader.ReadLine(); 
					if (header == null) return todos;
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						List<string> parts = ParseCsvLine(line, ';');
						if (parts.Count == 4)
						{
							string text = parts[1].Replace("\\n", "\n");
							if (Enum.TryParse<TodoStatus>(parts[2], true, out TodoStatus status) && DateTime.TryParse(parts[3], out DateTime lastUpdate))
							{
								todos.Add(new TodoItem(text, status, lastUpdate));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при загрузке задач из '{filePath}': {ex.Message}");
			}
			return todos;
		}
		private static List<string> ParseCsvLine(string line, char separator)
		{
			var parts = new List<string>();
			bool inQuote = false;
			int start = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '"')
				{
					if (i + 1 < line.Length && line[i + 1] == '"')
					{
						i++;
					}
					else
					{
						inQuote = !inQuote;
					}
				}
				else if (line[i] == separator && !inQuote)
				{
					string part = line.Substring(start, i - start);
					parts.Add(TrimQuotesAndUnescape(part));
					start = i + 1;
				}
			}
			string lastPart = line.Substring(start);
			parts.Add(TrimQuotesAndUnescape(lastPart));
			return parts;
		}
		private static string TrimQuotesAndUnescape(string input)
		{
			if (input.StartsWith("\"") && input.EndsWith("\"") && input.Length >= 2)
			{
				return input.Substring(1, input.Length - 2).Replace("\"\"", "\"");
			}
			return input;
		}
	}
}