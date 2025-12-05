using System;
namespace TodoList
{
	public class TodoItem
	{
		private string text;
		public TodoStatus Status { get; private set; }
		private DateTime lastUpdate;
		public TodoItem(string text)
		{
			this.text = text;
			this.Status = TodoStatus.NotStarted;
			this.lastUpdate = DateTime.Now;
		}
		public TodoItem(string text, TodoStatus status, DateTime lastUpdate)
		{
			this.text = text;
			this.Status = status;
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
			this.Status = newStatus;
			this.lastUpdate = DateTime.Now;
		}
		public void UpdateText(string newText)
		{
			text = newText;
			lastUpdate = DateTime.Now;
		}
		public string GetShortInfo()
		{
			string shortText = text.Length > 30 ? text.Substring(0, 27) + "..." : text;
			string status = this.Status.ToString();
			return $"{shortText} ({status}, обновлено {lastUpdate:dd.MM.yyyy HH:mm})";
		}
		public string GetFullInfo()
		{
			string status = this.Status.ToString();
			return $"Текст задачи:\n{text}\nСтатус: {status}\nДата последнего изменения: {lastUpdate:dd.MM.yyyy HH:mm}";
		}
	}
}