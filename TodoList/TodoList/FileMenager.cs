using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using TodoList.Exceptions; 
namespace TodoList
{
	public class FileManager : IDataStorage
	{
		private readonly string _dataDirectory;
		private readonly string _profilesPath;
		private static readonly byte[] _aesKey = { 0x45, 0x12, 0x99, 0x1C, 0x7B, 0x3A, 0x2F, 0x88, 0x10, 0x44, 0xBB, 0x5D, 0x9A, 0x8E, 0x22, 0x77, 0x11, 0x55, 0x33, 0x66, 0x99, 0xAA, 0xCC, 0xEE, 0xDD, 0xFF, 0x00, 0x12, 0x34, 0x56, 0x78, 0x90 };
		private static readonly byte[] _aesIv = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
		public FileManager(string dataDirectory)
		{
			_dataDirectory = dataDirectory;
			_profilesPath = Path.Combine(_dataDirectory, "profiles.csv");
			if (!Directory.Exists(_dataDirectory))
			{
				Directory.CreateDirectory(_dataDirectory);
			}
		}
		public void SaveProfiles(IEnumerable<Profile> profiles)
		{
			if (profiles == null) return;
			try
			{
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(_profilesPath, FileMode.Create, FileAccess.Write))
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
			catch (CryptographicException ex) { throw new DataEncryptionException("Ошибка шифрования при сохранении профилей.", ex); }
			catch (UnauthorizedAccessException ex) { throw new DataAccessException("Нет прав доступа для сохранения профилей. Файл заблокирован.", ex); }
			catch (IOException ex) { throw new DataAccessException("Ошибка ввода-вывода (файл занят другим процессом).", ex); }
			catch (Exception ex) { throw new DataStorageException($"Непредвиденная ошибка при сохранении профилей: {ex.Message}", ex); }
		}
		public IEnumerable<Profile> LoadProfiles()
		{
			var profiles = new List<Profile>();
			if (!File.Exists(_profilesPath)) return profiles;
			try
			{
				using (Aes aes = Aes.Create())
				{
					aes.Key = _aesKey;
					aes.IV = _aesIv;
					using (FileStream fs = new FileStream(_profilesPath, FileMode.Open, FileAccess.Read))
					using (BufferedStream bs = new BufferedStream(fs))
					using (CryptoStream cs = new CryptoStream(bs, aes.CreateDecryptor(), CryptoStreamMode.Read))
					using (StreamReader reader = new StreamReader(cs))
					{
						string header = reader.ReadLine();
						if (header == null) throw new DataCorruptionException("Файл профилей пуст.");
						if (!header.StartsWith("Id;Login")) throw new DataCorruptionException("Файл профилей поврежден: неверный заголовок.");
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							if (string.IsNullOrWhiteSpace(line)) continue;
							string[] parts = line.Split(';');
							if (parts.Length != 6) throw new DataCorruptionException("Файл профилей поврежден: нарушена структура столбцов.");
							if (!Guid.TryParse(parts[0], out Guid id)) throw new DataCorruptionException($"Поврежден ID пользователя: {parts[1]}");
							if (!int.TryParse(parts[5], out int birthYear)) throw new DataCorruptionException($"Поврежден год рождения пользователя: {parts[1]}");
							profiles.Add(new Profile(id, parts[1], parts[2], parts[3], parts[4], birthYear));
						}
					}
				}
			}
			catch (CryptographicException ex) { throw new DataEncryptionException("Ошибка расшифровки профилей. Неверный ключ, либо файл повреждён извне.", ex); }
			catch (UnauthorizedAccessException ex) { throw new DataAccessException("Нет прав доступа для чтения файла профилей.", ex); }
			catch (IOException ex) { throw new DataAccessException("Ошибка ввода-вывода при чтении файла профилей (файл занят).", ex); }
			catch (DataStorageException) { throw; }
			catch (Exception ex) { throw new DataStorageException($"Непредвиденная ошибка при загрузке профилей: {ex.Message}", ex); }

			return profiles;
		}
		public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
		{
			if (todos == null) return;
			string filePath = Path.Combine(_dataDirectory, $"todos_{userId}.csv");
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
						int index = 0;
						foreach (var item in todos)
						{
							string processedText = item.Text.Replace("\n", "\\n").Replace("\"", "\"\"");
							if (processedText.Contains(";")) processedText = $"\"{processedText}\"";

							writer.WriteLine($"{index};{processedText};{item.Status.ToString()};{item.LastUpdate:o}");
							index++;
						}
					}
				}
			}
			catch (CryptographicException ex) { throw new DataEncryptionException("Ошибка шифрования при сохранении задач.", ex); }
			catch (UnauthorizedAccessException ex) { throw new DataAccessException("Нет прав доступа для сохранения задач.", ex); }
			catch (IOException ex) { throw new DataAccessException("Ошибка ввода-вывода (файл занят другим процессом).", ex); }
			catch (Exception ex) { throw new DataStorageException($"Непредвиденная ошибка при сохранении задач: {ex.Message}", ex); }
		}
		public IEnumerable<TodoItem> LoadTodos(Guid userId)
		{
			var todos = new List<TodoItem>();
			string filePath = Path.Combine(_dataDirectory, $"todos_{userId}.csv");
			if (!File.Exists(filePath)) return todos;
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
						if (header == null) throw new DataCorruptionException("Файл задач пуст.");
						if (!header.StartsWith("Index;Text;Status")) throw new DataCorruptionException("Файл задач поврежден: неверный заголовок.");
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							if (string.IsNullOrWhiteSpace(line)) continue;
							List<string> parts = ParseCsvLine(line, ';');
							if (parts.Count != 4) throw new DataCorruptionException("Нарушена структура данных задачи (неверное количество столбцов).");
							string text = parts[1].Replace("\\n", "\n");
							if (!Enum.TryParse<TodoStatus>(parts[2], true, out TodoStatus status))
								throw new DataCorruptionException($"Поврежден статус задачи: {parts[2]}");
							if (!DateTime.TryParse(parts[3], out DateTime lastUpdate))
								throw new DataCorruptionException($"Повреждена дата обновления задачи: {parts[3]}");
							todos.Add(new TodoItem(text, status, lastUpdate));
						}
					}
				}
			}
			catch (CryptographicException ex) { throw new DataEncryptionException("Ошибка расшифровки задач. Неверный ключ или файл повреждён.", ex); }
			catch (UnauthorizedAccessException ex) { throw new DataAccessException("Нет прав доступа для чтения файла задач.", ex); }
			catch (IOException ex) { throw new DataAccessException("Ошибка ввода-вывода при чтении файла задач (файл занят).", ex); }
			catch (DataStorageException) { throw; }
			catch (Exception ex) { throw new DataStorageException($"Непредвиденная ошибка при загрузке задач: {ex.Message}", ex); }
			return todos;
		}
		private static List<string> ParseCsvLine(string line, char separator)
		{
			var parts = new List<string>();
			bool inQuote = false; int start = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '"')
				{
					if (i + 1 < line.Length && line[i + 1] == '"') i++; else inQuote = !inQuote;
				}
				else if (line[i] == separator && !inQuote)
				{
					parts.Add(TrimQuotesAndUnescape(line.Substring(start, i - start)));
					start = i + 1;
				}
			}
			parts.Add(TrimQuotesAndUnescape(line.Substring(start)));
			return parts;
		}
		private static string TrimQuotesAndUnescape(string input)
		{
			if (input.StartsWith("\"") && input.EndsWith("\"") && input.Length >= 2)
				return input.Substring(1, input.Length - 2).Replace("\"\"", "\"");
			return input;
		}
	}
}