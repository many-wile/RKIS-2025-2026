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
				List<string> linesToWrite = new List<string>();
				linesToWrite.Add("Id;Login;Password;FirstName;LastName;BirthYear");
				foreach (var profile in profiles)
				{
					linesToWrite.Add(profile.ToCsvString());
				}
				File.WriteAllLines(filePath, linesToWrite);
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
				string[] lines = File.ReadAllLines(filePath);
				if (lines.Length <= 1)
				{
					return profiles;
				}
				for (int i = 1; i < lines.Length; i++)
				{
					string[] parts = lines[i].Split(';');
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
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при загрузке профилей из '{filePath}': {ex.Message}");
			}
			return profiles;
		}
		public static void SaveTodos(TodoList todos, string filePath)
		{
			try
			{
				List<string> linesToWrite = new List<string>();
				linesToWrite.Add("Index;Text;Status;LastUpdate");
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
						linesToWrite.Add($"{i};{processedText};{item.Status.ToString()};{item.LastUpdate:o}");
					}
				}
				File.WriteAllLines(filePath, linesToWrite);
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
				string[] lines = File.ReadAllLines(filePath);
				if (lines.Length <= 1)
				{
					return todos;
				}
				for (int i = 1; i < lines.Length; i++)
				{
					List<string> parts = ParseCsvLine(lines[i], ';');
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