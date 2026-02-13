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
				Console.WriteLine("Ничего не найдено");
				return;
			}
			var query = todoList.AsEnumerable();
			if (!string.IsNullOrEmpty(_contains))
				query = query.Where(x => x.Text.IndexOf(_contains, StringComparison.OrdinalIgnoreCase) >= 0);
			if (!string.IsNullOrEmpty(_startsWith))
				query = query.Where(x => x.Text.Trim().StartsWith(_startsWith, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(_endsWith))
				query = query.Where(x => x.Text.Trim().EndsWith(_endsWith, StringComparison.OrdinalIgnoreCase));
			if (_status.HasValue)
				query = query.Where(x => x.Status == _status.Value);
			if (_dateFrom.HasValue)
				query = query.Where(x => x.LastUpdate.Date >= _dateFrom.Value);
			if (_dateTo.HasValue)
				query = query.Where(x => x.LastUpdate.Date <= _dateTo.Value);
			if (!string.IsNullOrEmpty(_sortBy))
			{
				if (_sortBy == "text")
					query = _desc ? query.OrderByDescending(x => x.Text) : query.OrderBy(x => x.Text);
				else if (_sortBy == "date")
					query = _desc ? query.OrderByDescending(x => x.LastUpdate) : query.OrderBy(x => x.LastUpdate);
			}
			if (_top.HasValue)
				query = query.Take(_top.Value);
			var results = query.ToList();
			if (results.Any())
			{
				Console.WriteLine($"Найдено задач: {results.Count}\n");
				var resultList = new TodoList(results);
				resultList.View(true, true, true);
			}
			else
			{
				Console.WriteLine("Ничего не найдено");
			}
		}
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
							else { Console.WriteLine($"Ошибка: Статус '{val}' не существует."); return false; }
							break;
						case "--from":
							if (DateTime.TryParse(val, out var df)) _dateFrom = df.Date;
							else { Console.WriteLine("Ошибка: Некорректная дата --from."); return false; }
							break;
						case "--to":
							if (DateTime.TryParse(val, out var dt)) _dateTo = dt.Date;
							else { Console.WriteLine("Ошибка: Некорректная дата --to."); return false; }
							break;
						case "--sort":
							if (val == "text" || val == "date") _sortBy = val;
							else { Console.WriteLine("Ошибка: sort должен быть 'text' или 'date'."); return false; }
							break;
						case "--top":
							if (int.TryParse(val, out int t) && t > 0) _top = t;
							else { Console.WriteLine("Ошибка: --top должен быть числом > 0."); return false; }
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