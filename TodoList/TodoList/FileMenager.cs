using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace TodoList
{
	static class FileManager
	{
		private static readonly byte[] _aesKey = { 0x45, 0x12, 0x99, 0x1C, 0x7B, 0x3A, 0x2F, 0x88, 0x10, 0x44, 0xBB, 0x5D, 0x9A, 0x8E, 0x22, 0x77, 0x11, 0x55, 0x33, 0x66, 0x99, 0xAA, 0xCC, 0xEE, 0xDD, 0xFF, 0x00, 0x12, 0x34, 0x56, 0x78, 0x90 };
		private static readonly byte[] _aesIv = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
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
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
					using (BufferedStream bs = new BufferedStream(fs))
					using (CryptoStream cs = new CryptoStream(bs, aes.CreateEncryptor(), CryptoStreamMode.Write))
					using (StreamWriter writer = new StreamWriter(cs))
					{
						writer.WriteLine("Id;Login;Password;FirstName;LastName;BirthYear");
						foreach (var profile in profiles)
						{
							writer.WriteLine(profile.ToCsvString());
						}
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
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					using (BufferedStream bs = new BufferedStream(fs))
					using (CryptoStream cs = new CryptoStream(bs, aes.CreateDecryptor(), CryptoStreamMode.Read))
					using (StreamReader reader = new StreamReader(cs))
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
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
					using (BufferedStream bs = new BufferedStream(fs))
					using (CryptoStream cs = new CryptoStream(bs, aes.CreateEncryptor(), CryptoStreamMode.Write))
					using (StreamWriter writer = new StreamWriter(cs))
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
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					using (BufferedStream bs = new BufferedStream(fs))
					using (CryptoStream cs = new CryptoStream(bs, aes.CreateDecryptor(), CryptoStreamMode.Read))
					using (StreamReader reader = new StreamReader(cs))
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