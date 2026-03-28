using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TodoList.Exceptions;

namespace TodoList
{
	public class FileManager : IDataStorage
	{
		private readonly string _dataDirectory;
		private readonly string _profilesPath;

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
			if (profiles == null)
			{
				return;
			}

			try
			{
				using Aes aes = PayloadCrypto.CreateAes();
				using FileStream fs = new FileStream(_profilesPath, FileMode.Create, FileAccess.Write);
				using BufferedStream bs = new BufferedStream(fs);
				using CryptoStream cs = new CryptoStream(bs, aes.CreateEncryptor(), CryptoStreamMode.Write);
				using StreamWriter writer = new StreamWriter(cs);

				writer.WriteLine("Id;Login;Password;FirstName;LastName;BirthYear");
				foreach (Profile profile in profiles)
				{
					writer.WriteLine(profile.ToCsvString());
				}
			}
			catch (CryptographicException ex)
			{
				throw new DataEncryptionException("Encryption error while saving profiles.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new DataAccessException("Access denied while saving profiles.", ex);
			}
			catch (IOException ex)
			{
				throw new DataAccessException("I/O error while saving profiles.", ex);
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Unexpected error while saving profiles: {ex.Message}", ex);
			}
		}

		public IEnumerable<Profile> LoadProfiles()
		{
			List<Profile> profiles = new List<Profile>();
			if (!File.Exists(_profilesPath))
			{
				return profiles;
			}

			try
			{
				using Aes aes = PayloadCrypto.CreateAes();
				using FileStream fs = new FileStream(_profilesPath, FileMode.Open, FileAccess.Read);
				using BufferedStream bs = new BufferedStream(fs);
				using CryptoStream cs = new CryptoStream(bs, aes.CreateDecryptor(), CryptoStreamMode.Read);
				using StreamReader reader = new StreamReader(cs);

				string? header = reader.ReadLine();
				if (header == null)
				{
					throw new DataCorruptionException("Profiles file is empty.");
				}

				if (!header.StartsWith("Id;Login", StringComparison.Ordinal))
				{
					throw new DataCorruptionException("Profiles file is corrupted: invalid header.");
				}

				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line))
					{
						continue;
					}

					string[] parts = line.Split(';');
					if (parts.Length != 6)
					{
						throw new DataCorruptionException("Profiles file is corrupted: invalid column count.");
					}

					if (!Guid.TryParse(parts[0], out Guid id))
					{
						throw new DataCorruptionException($"Profile id is corrupted for login: {parts[1]}");
					}

					if (!int.TryParse(parts[5], out int birthYear))
					{
						throw new DataCorruptionException($"Birth year is corrupted for login: {parts[1]}");
					}

					profiles.Add(new Profile(id, parts[1], parts[2], parts[3], parts[4], birthYear));
				}
			}
			catch (CryptographicException ex)
			{
				throw new DataEncryptionException("Decryption error while loading profiles.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new DataAccessException("Access denied while loading profiles.", ex);
			}
			catch (IOException ex)
			{
				throw new DataAccessException("I/O error while loading profiles.", ex);
			}
			catch (DataStorageException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Unexpected error while loading profiles: {ex.Message}", ex);
			}

			return profiles;
		}

		public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
		{
			if (todos == null)
			{
				return;
			}

			string filePath = Path.Combine(_dataDirectory, $"todos_{userId}.csv");
			try
			{
				using Aes aes = PayloadCrypto.CreateAes();
				using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				using BufferedStream bs = new BufferedStream(fs);
				using CryptoStream cs = new CryptoStream(bs, aes.CreateEncryptor(), CryptoStreamMode.Write);
				using StreamWriter writer = new StreamWriter(cs);

				writer.WriteLine("Index;Text;Status;LastUpdate");
				int index = 0;
				foreach (TodoItem item in todos)
				{
					string processedText = item.Text.Replace("\n", "\\n").Replace("\"", "\"\"");
					if (processedText.Contains(';'))
					{
						processedText = $"\"{processedText}\"";
					}

					writer.WriteLine($"{index};{processedText};{item.Status};{item.LastUpdate:o}");
					index++;
				}
			}
			catch (CryptographicException ex)
			{
				throw new DataEncryptionException("Encryption error while saving todos.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new DataAccessException("Access denied while saving todos.", ex);
			}
			catch (IOException ex)
			{
				throw new DataAccessException("I/O error while saving todos.", ex);
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Unexpected error while saving todos: {ex.Message}", ex);
			}
		}

		public IEnumerable<TodoItem> LoadTodos(Guid userId)
		{
			List<TodoItem> todos = new List<TodoItem>();
			string filePath = Path.Combine(_dataDirectory, $"todos_{userId}.csv");
			if (!File.Exists(filePath))
			{
				return todos;
			}

			try
			{
				using Aes aes = PayloadCrypto.CreateAes();
				using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				using BufferedStream bs = new BufferedStream(fs);
				using CryptoStream cs = new CryptoStream(bs, aes.CreateDecryptor(), CryptoStreamMode.Read);
				using StreamReader reader = new StreamReader(cs);

				string? header = reader.ReadLine();
				if (header == null)
				{
					throw new DataCorruptionException("Todos file is empty.");
				}

				if (!header.StartsWith("Index;Text;Status", StringComparison.Ordinal))
				{
					throw new DataCorruptionException("Todos file is corrupted: invalid header.");
				}

				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line))
					{
						continue;
					}

					List<string> parts = ParseCsvLine(line, ';');
					if (parts.Count != 4)
					{
						throw new DataCorruptionException("Todos file is corrupted: invalid column count.");
					}

					string text = parts[1].Replace("\\n", "\n");
					if (!Enum.TryParse(parts[2], true, out TodoStatus status))
					{
						throw new DataCorruptionException($"Todo status is corrupted: {parts[2]}");
					}

					if (!DateTime.TryParse(parts[3], out DateTime lastUpdate))
					{
						throw new DataCorruptionException($"Todo date is corrupted: {parts[3]}");
					}

					todos.Add(new TodoItem(text, status, lastUpdate));
				}
			}
			catch (CryptographicException ex)
			{
				throw new DataEncryptionException("Decryption error while loading todos.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new DataAccessException("Access denied while loading todos.", ex);
			}
			catch (IOException ex)
			{
				throw new DataAccessException("I/O error while loading todos.", ex);
			}
			catch (DataStorageException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Unexpected error while loading todos: {ex.Message}", ex);
			}

			return todos;
		}

		private static List<string> ParseCsvLine(string line, char separator)
		{
			List<string> parts = new List<string>();
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
					parts.Add(TrimQuotesAndUnescape(line.Substring(start, i - start)));
					start = i + 1;
				}
			}

			parts.Add(TrimQuotesAndUnescape(line.Substring(start)));
			return parts;
		}

		private static string TrimQuotesAndUnescape(string input)
		{
			if (input.StartsWith('"') && input.EndsWith('"') && input.Length >= 2)
			{
				return input.Substring(1, input.Length - 2).Replace("\"\"", "\"");
			}

			return input;
		}
	}
}