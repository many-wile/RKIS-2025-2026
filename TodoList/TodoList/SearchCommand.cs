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

			var todos = AppInfo.CurrentUserTodos;
			if (todos.Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
				return;
			}
			var query = todos.Select((item, index) => new { Item = item, OriginalIndex = index + 1 });
			if (!string.IsNullOrEmpty(_contains))
				query = query.Where(x => x.Item.Text.IndexOf(_contains, StringComparison.OrdinalIgnoreCase) >= 0);
			if (!string.IsNullOrEmpty(_startsWith))
				query = query.Where(x => x.Item.Text.Trim().StartsWith(_startsWith, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(_endsWith))
				query = query.Where(x => x.Item.Text.Trim().EndsWith(_endsWith, StringComparison.OrdinalIgnoreCase));
			if (_dateFrom.HasValue)
				query = query.Where(x => x.Item.LastUpdate.Date >= _dateFrom.Value);
			if (_dateTo.HasValue)
				query = query.Where(x => x.Item.LastUpdate.Date <= _dateTo.Value);
			if (_status.HasValue)
				query = query.Where(x => x.Item.Status == _status.Value);
			if (_sortBy == "text")
			{
				query = _desc
					? query.OrderByDescending(x => x.Item.Text)
					: query.OrderBy(x => x.Item.Text);
			}
			else if (_sortBy == "date")
			{
				query = _desc
					? query.OrderByDescending(x => x.Item.LastUpdate)
					: query.OrderBy(x => x.Item.LastUpdate);
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
			if (results.Count > 0)
			{
				Console.WriteLine($"Найдено задач: {results.Count}");
				foreach (var res in results)
				{
					Console.WriteLine($"[{res.OriginalIndex}] {res.Item.GetShortInfo()}");
				}
			}
			else
			{
				Console.WriteLine("Задачи, удовлетворяющие условиям, не найдены.");
			}
		}
		public void Unexecute()
		{
		}
		private bool ParseArguments()
		{
			var args = SplitArgs(_rawArgs);

			for (int i = 0; i < args.Count; i++)
			{
				string arg = args[i].ToLower();
				if (i + 1 < args.Count)
				{
					string nextVal = args[i + 1];

					switch (arg)
					{
						case "--contains":
							_contains = nextVal;
							i++; continue;
						case "--starts-with":
							_startsWith = nextVal;
							i++; continue;
						case "--ends-with":
							_endsWith = nextVal;
							i++; continue;
						case "--status":
							if (Enum.TryParse<TodoStatus>(nextVal, true, out var st))
							{
								_status = st;
							}
							else
							{
								Console.WriteLine($"Ошибка: Неизвестный статус '{nextVal}'.");
								return false;
							}
							i++; continue;
						case "--from":
							if (DateTime.TryParse(nextVal, out var df)) _dateFrom = df.Date;
							else { Console.WriteLine("Ошибка: Неверный формат даты --from (ожидается yyyy-MM-dd)."); return false; }
							i++; continue;
						case "--to":
							if (DateTime.TryParse(nextVal, out var dt)) _dateTo = dt.Date;
							else { Console.WriteLine("Ошибка: Неверный формат даты --to (ожидается yyyy-MM-dd)."); return false; }
							i++; continue;
						case "--sort":
							if (nextVal.ToLower() == "text" || nextVal.ToLower() == "date") _sortBy = nextVal.ToLower();
							else { Console.WriteLine("Ошибка: sort может быть 'text' или 'date'."); return false; }
							i++; continue;
						case "--top":
							if (int.TryParse(nextVal, out int n)) _top = n;
							else { Console.WriteLine("Ошибка: --top должно быть числом."); return false; }
							i++; continue;
					}
				}
				if (arg == "--desc")
				{
					_desc = true;
				}
				else if (i == 0 && !arg.StartsWith("--"))
				{
					_contains = args[i];
				}
			}
			return true;
		}
		private List<string> SplitArgs(string input)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(input)) return result;
			bool inQuotes = false;
			StringBuilder current = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (c == '"')
				{
					inQuotes = !inQuotes;
				}
				else if (c == ' ' && !inQuotes)
				{
					if (current.Length > 0)
					{
						result.Add(current.ToString());
						current.Clear();
					}
				}
				else
				{
					current.Append(c);
				}
			}
			if (current.Length > 0) result.Add(current.ToString());
			return result;
		}
	}
}