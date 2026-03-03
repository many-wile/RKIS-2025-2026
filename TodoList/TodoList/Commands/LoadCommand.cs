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
		private async Task RunAsync()
		{
			int startRow = Console.CursorTop;
			for (int i = 0; i < _downloadsCount; i++)
			{
				Console.WriteLine();
			}
			List<Task> tasks = new List<Task>();
			for (int index = 0; index < _downloadsCount; index++)
			{
				int row = startRow + index;
				tasks.Add(DownloadAsync(row));
			}
			await Task.WhenAll(tasks);
			lock (_consoleLock)
			{
				Console.SetCursorPosition(0, startRow + _downloadsCount);
				Console.WriteLine("Все загрузки завершены.");
			}
		}
		private async Task DownloadAsync(int row)
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			for (int i = 0; i <= _downloadSize; i++)
			{
				double percentage = _downloadSize == 0 ? 100 : ((double)i / _downloadSize) * 100;
				int percentInt = (int)percentage;
				int filledBars = percentInt / 5;
				if (filledBars > 20) filledBars = 20;
				int emptyBars = 20 - filledBars;
				string bar = $"[{new string('#', filledBars)}{new string('-', emptyBars)}] {percentInt}%";
				lock (_consoleLock)
				{
					Console.SetCursorPosition(0, row);
					Console.Write(bar);
				}
				if (i < _downloadSize)
				{
					await Task.Delay(random.Next(10, 50));
				}
			}
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
	}
}