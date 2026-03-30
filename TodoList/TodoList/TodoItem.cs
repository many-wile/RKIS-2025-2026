using System;

namespace TodoList
{
	public class TodoItem
	{
		private string text;
		private readonly IClock _clock;
		public TodoStatus Status { get; private set; }
		private DateTime lastUpdate;

		public TodoItem(string text, IClock? clock = null)
		{
			_clock = clock ?? new SystemClock();
			this.text = text;
			Status = TodoStatus.NotStarted;
			lastUpdate = _clock.Now;
		}

		public TodoItem(string text, TodoStatus status, DateTime lastUpdate, IClock? clock = null)
		{
			_clock = clock ?? new SystemClock();
			this.text = text;
			Status = status;
			this.lastUpdate = lastUpdate;
		}

		public string Text
		{
			get { return text; }
		}

		public DateTime LastUpdate
		{
			get { return lastUpdate; }
		}

		public void ChangeStatus(TodoStatus newStatus)
		{
			Status = newStatus;
			lastUpdate = _clock.Now;
		}

		public void UpdateText(string newText)
		{
			text = newText;
			lastUpdate = _clock.Now;
		}

		public string GetShortInfo()
		{
			string shortText = text.Length > 30 ? text.Substring(0, 27) + "..." : text;
			string status = Status.ToString();
			return $"{shortText} ({status}, обновлено {lastUpdate:dd.MM.yyyy HH:mm})";
		}

		public string GetFullInfo()
		{
			string status = Status.ToString();
			return $"Текст задачи:\n{text}\nСтатус: {status}\nДата последнего изменения: {lastUpdate:dd.MM.yyyy HH:mm}";
		}
	}
}
