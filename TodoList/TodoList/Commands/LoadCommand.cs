using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Exceptions;
namespace TodoList.Commands
{
	public class LoadCommand : ICommand
	{
		private readonly string _args;
		private int _downloadsCount;
		private int _downloadSize;
		private static readonly object _consoleLock = new object();
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
			for (int i = 0; i < _downloadsCount; i++)
			{
				Console.WriteLine();
			}
			int startRow = Console.CursorTop - _downloadsCount;
			List<Task> tasks = new List<Task>();
			for (int i = 0; i < _downloadsCount; i++)
			{
				int row = startRow + i;
				tasks.Add(DownloadAsync(row, i + 1));
			}
			await Task.WhenAll(tasks);
			lock (_consoleLock)
			{
				Console.SetCursorPosition(0, startRow + _downloadsCount);
				Console.WriteLine("\nВсе загрузки завершены.");
			}
		}
		private async Task DownloadAsync(int row, int downloadNumber)
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			for (int i = 0; i <= _downloadSize; i++)
			{
				double percentage = ((double)i / _downloadSize) * 100;
				int percentInt = (int)percentage;
				int filledBars = percentInt / 5;
				if (filledBars > 20) filledBars = 20;
				int emptyBars = 20 - filledBars;
				string bar = $"Поток {downloadNumber,2}: [{new string('#', filledBars)}{new string('-', emptyBars)}] {i}/{_downloadSize} ({percentInt,3}%)";
				lock (_consoleLock)
				{
					Console.SetCursorPosition(0, row);
					Console.Write(bar.PadRight(Console.WindowWidth - 1));
				}

				if (i < _downloadSize)
				{
					await Task.Delay(random.Next(10, 50));
				}
			}
		}
	}
}