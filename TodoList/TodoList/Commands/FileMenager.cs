using System;
using System.IO;
using System.Collections.Generic;
namespace TodoList
{
    static class FileManager
    {
        public static void EnsureDataDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Console.WriteLine($"Создана папка для данных: {dirPath}");
            }
        }
        public static void SaveProfile(Profile profile, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, profile.GetRawInfo());
                Console.WriteLine($"Профиль пользователя сохранен в {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении профиля: {ex.Message}");
            }
        }
        public static Profile LoadProfile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл профиля не найден: {filePath}. Будет создан новый профиль.");
                return null;
            }
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 2)
                {
                    string fullName = lines[0];
                    if (DateTime.TryParse(lines[1], out DateTime birthDate))
                    {
                        Console.WriteLine($"Профиль пользователя загружен из {filePath}");
                        return new Profile(fullName, birthDate);
                    }
                }
                Console.WriteLine($"Неверный формат данных в файле профиля: {filePath}. Будет создан новый профиль.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке профиля: {ex.Message}. Будет создан новый профиль.");
                return null;
            }
        }
        public static void SaveTodos(TodoList todos, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("Index;Text;IsDone;LastUpdate");
                    for (int i = 0; i < todos.count; i++)
                    {
                        TodoItem item = todos.GetItem(i + 1);
                        if (item != null)
                        {
                            string processedText = item.Text.Replace("\n", "\\n");
                            processedText = processedText.Replace("\"", "\"\"");
                            if (processedText.Contains(";") || processedText.Contains("\\n"))
                            {
                                processedText = $"\"{processedText}\"";
                            }
                            sw.WriteLine($"{i};{processedText};{item.IsDone};{item.LastUpdate:o}");
                        }
                    }
                }
                Console.WriteLine($"Задачи сохранены в {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
            }
        }
        public static TodoList LoadTodos(string filePath)
        {
            TodoList todos = new TodoList();
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл задач не найден: {filePath}. Будет создан пустой список задач.");
                return todos;
            }
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length <= 1)
                {
                    Console.WriteLine($"Файл задач пуст или содержит только заголовок: {filePath}.");
                    return todos;
                }
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    List<string> parts = ParseCsvLine(line, ';');
                    if (parts.Count == 4)
                    {
                        string text = parts[1];
                        string isDoneString = parts[2];
                        string lastUpdateString = parts[3];
                        if (bool.TryParse(isDoneString, out bool isDone) && DateTime.TryParse(lastUpdateString, out DateTime lastUpdate))
                        {
                            text = text.Replace("\\n", "\n");

                            TodoItem item = new TodoItem(text);
                            item.SetLoadedState(isDone, lastUpdate);
                            todos.Add(item);
                        }
                        else
                        {
                            Console.WriteLine($"Предупреждение: Неверный формат данных в строке: {line}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Предупреждение: Неверное количество полей в строке: {line}");
                    }
                }
                Console.WriteLine($"Задачи загружены из {filePath}");
                return todos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}. Будет создан пустой список задач.");
                return new TodoList();
            }
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