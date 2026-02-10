using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoList.Commands
{
	public class SearchCommand : ICommand
	{
		private string _rawArgs;
		private string _contains;
		private string _startsWith;
		private string _endsWith;
		private DateTime? _dateFrom;
		private DateTime? _dateTo;
		private TodoStatus? _status;
		private string _sortBy;
		private bool _desc;
		private int? _top;
		public SearchCommand(string args)
		{
			_rawArgs = args;
		}
		public void Execute()
		{
			if (!ParseArguments())
			{
				return;
			}
			var todoList = AppInfo.CurrentUserTodos;
			if (todoList == null || todoList.Count == 0)
			{
				Console.WriteLine("Ничего не найдено");
				return;
			}
			var query = todoList.Select((item, index) => new { Item = item, OriginalIndex = index + 1 });
			if (!string.IsNullOrEmpty(_contains))
				query = query.Where(x => x.Item.Text.IndexOf(_contains, StringComparison.OrdinalIgnoreCase) >= 0);
			if (!string.IsNullOrEmpty(_startsWith))
				query = query.Where(x => x.Item.Text.Trim().StartsWith(_startsWith, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(_endsWith))
				query = query.Where(x => x.Item.Text.Trim().EndsWith(_endsWith, StringComparison.OrdinalIgnoreCase));
			if (_status.HasValue)
				query = query.Where(x => x.Item.Status == _status.Value);
			if (_dateFrom.HasValue)
				query = query.Where(x => x.Item.LastUpdate.Date >= _dateFrom.Value);
			if (_dateTo.HasValue)
				query = query.Where(x => x.Item.LastUpdate.Date <= _dateTo.Value);
			if (!string.IsNullOrEmpty(_sortBy))
			{
				if (_sortBy == "text")
					query = _desc ? query.OrderByDescending(x => x.Item.Text) : query.OrderBy(x => x.Item.Text);
				else if (_sortBy == "date")
					query = _desc ? query.OrderByDescending(x => x.Item.LastUpdate) : query.OrderBy(x => x.Item.LastUpdate);
			}
			else if (_desc)
			{
				query = query.OrderByDescending(x => x.OriginalIndex);
			}
			if (_top.HasValue)
			{
				query = query.Take(_top.Value);
			}
			var results = query.ToList();
			if (results.Any())
			{
				Console.WriteLine($"Найдено задач: {results.Count}\n");
				string format = "{0,-6} | {1,-30} | {2,-12} | {3,-18}";
				Console.WriteLine(string.Format(format, "Index", "Text", "Status", "LastUpdate"));
				Console.WriteLine(new string('-', 75));
				foreach (var res in results)
				{
					string text = res.Item.Text.Replace("\n", " ");
					if (text.Length > 27) text = text.Substring(0, 27) + "...";
					Console.WriteLine(string.Format(format,
						res.OriginalIndex,
						text,
						res.Item.Status,
						res.Item.LastUpdate.ToString("yyyy-MM-dd HH:mm")));
				}
				Console.WriteLine(new string('-', 75));
			}
			else
			{
				Console.WriteLine("Ничего не найдено");
			}
		}
		public void Unexecute() { }
		private bool ParseArguments()
		{
			var args = SplitArgs(_rawArgs);
			for (int i = 0; i < args.Count; i++)
			{
				string arg = args[i].ToLower();
				if (i + 1 < args.Count)
				{
					string val = args[i + 1];
					bool consumed = true;
					switch (arg)
					{
						case "--contains": _contains = val; break;
						case "--starts-with": _startsWith = val; break;
						case "--ends-with": _endsWith = val; break;
						case "--status":
							if (Enum.TryParse<TodoStatus>(val, true, out var st)) _status = st;
							else
							{
								Console.WriteLine($"Ошибка: Статус '{val}' не существует.");
								return false;
							}
							break;
						case "--from":
							if (DateTime.TryParse(val, out var df)) _dateFrom = df.Date;
							else
							{
								Console.WriteLine($"Ошибка: Некорректная дата '{val}' для флага --from. Используйте формат yyyy-MM-dd.");
								return false;
							}
							break;

						case "--to":
							if (DateTime.TryParse(val, out var dt)) _dateTo = dt.Date;
							else
							{
								Console.WriteLine($"Ошибка: Некорректная дата '{val}' для флага --to. Используйте формат yyyy-MM-dd.");
								return false;
							}
							break;
						case "--sort":
							if (val == "text" || val == "date") _sortBy = val;
							else
							{
								Console.WriteLine("Ошибка: Параметр sort должен быть 'text' или 'date'.");
								return false;
							}
							break;
						case "--top":
							if (int.TryParse(val, out int t))
							{
								if (t > 0) _top = t;
								else
								{
									Console.WriteLine("Ошибка: Значение --top должно быть больше 0.");
									return false;
								}
							}
							else
							{
								Console.WriteLine($"Ошибка: Значение '{val}' для --top не является целым числом.");
								return false;
							}
							break;
						default: consumed = false; break;
					}
					if (consumed) { i++; continue; }
				}
				if (arg == "--desc") _desc = true;
				else if (i == 0 && !arg.StartsWith("--")) _contains = args[i];
			}
			return true;
		}
		private List<string> SplitArgs(string input)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(input)) return result;

			bool inQuotes = false;
			StringBuilder sb = new StringBuilder();

			foreach (char c in input)
			{
				if (c == '"') inQuotes = !inQuotes;
				else if (c == ' ' && !inQuotes)
				{
					if (sb.Length > 0) { result.Add(sb.ToString()); sb.Clear(); }
				}
				else sb.Append(c);
			}
			if (sb.Length > 0) result.Add(sb.ToString());
			return result;
		}
	}
}