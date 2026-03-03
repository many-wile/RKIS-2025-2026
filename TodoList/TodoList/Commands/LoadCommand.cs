using System;
using System.Threading.Tasks;
using TodoList.Exceptions;

namespace TodoList.Commands
{
	public class LoadCommand : ICommand
	{
		private readonly string _args;
		private int _downloadsCount;
		private int _downloadSize;
		public LoadCommand(string args)
		{
			_args = args;
		}
		public void Execute()
		{
			ParseArguments();
			RunAsync().Wait();
		}
		private void ParseArguments()
		{
			if (string.IsNullOrWhiteSpace(_args))
				throw new InvalidArgumentException("Аргументы не переданы. Использование: load <количество_скачиваний> <размер_скачиваний>");
			var parts = _args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
				throw new InvalidArgumentException("Неверное количество аргументов. Ожидалось 2 аргумента.");
			if (!int.TryParse(parts[0], out _downloadsCount) || !int.TryParse(parts[1], out _downloadSize))
				throw new InvalidArgumentException("Аргументы должны быть числами.");
			if (_downloadsCount <= 0 || _downloadSize <= 0)
				throw new InvalidArgumentException("Значения (количество и размер) должны быть больше 0.");
		}
		private async Task RunAsync()
		{
			Console.WriteLine($"[Mock] Подготовка к {_downloadsCount} загрузкам размером {_downloadSize}...");
			await Task.CompletedTask;
		}
	}
}