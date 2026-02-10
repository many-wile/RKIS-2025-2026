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
			if (!ParseArguments()) return;

			var todoList = AppInfo.CurrentUserTodos;
			if (todoList == null || todoList.Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
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
				query = query.Take(_top.Value);
			var results = query.ToList();
			if (results.Any())
			{
				Console.WriteLine($"Найдено задач: {results.Count}");
				Console.WriteLine();
				string headerFormat = "{0,-6} | {1,-30} | {2,-12} | {3,-18}";
				string header = string.Format(headerFormat, "Index", "Text", "Status", "LastUpdate");
				Console.WriteLine(header);
				Console.WriteLine(new string('-', header.Length));
				foreach (var res in results)
				{
					string rawText = res.Item.Text.Replace("\n", " ").Replace("\r", "");
					string formattedText = rawText.Length > 27
						? rawText.Substring(0, 27) + "..."
						: rawText;
					Console.WriteLine(headerFormat,
						res.OriginalIndex,
						formattedText,
						res.Item.Status,
						res.Item.LastUpdate.ToString("yyyy-MM-dd HH:mm"));
				}
				Console.WriteLine(new string('-', header.Length));
			}
			else
			{
				Console.WriteLine("Задачи, удовлетворяющие условиям, не найдены.");
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
					if (arg == "--contains") { _contains = val; i++; continue; }
					if (arg == "--starts-with") { _startsWith = val; i++; continue; }
					if (arg == "--ends-with") { _endsWith = val; i++; continue; }
					if (arg == "--status")
					{
						if (Enum.TryParse<TodoStatus>(val, true, out var st)) _status = st;
						else { Console.WriteLine($"Ошибка статуса: {val}"); return false; }
						i++; continue;
					}
					if (arg == "--from")
					{
						if (DateTime.TryParse(val, out var d)) _dateFrom = d.Date;
						else { Console.WriteLine("Неверный формат --from"); return false; }
						i++; continue;
					}
					if (arg == "--to")
					{
						if (DateTime.TryParse(val, out var d)) _dateTo = d.Date;
						else { Console.WriteLine("Неверный формат --to"); return false; }
						i++; continue;
					}
					if (arg == "--sort")
					{
						if (val == "text" || val == "date") _sortBy = val;
						else { Console.WriteLine("sort: text/date"); return false; }
						i++; continue;
					}
					if (arg == "--top")
					{
						if (int.TryParse(val, out int t)) _top = t;
						else { Console.WriteLine("--top должно быть числом"); return false; }
						i++; continue;
					}
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
				if (c == '\"') inQuotes = !inQuotes;
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