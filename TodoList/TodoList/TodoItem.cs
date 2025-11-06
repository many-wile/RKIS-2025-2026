using TodoList.Commands;
using System;

namespace TodoList
{
    internal class TodoItem
    {
        private string text;
        private bool isDone;
        private DateTime lastUpdate;
        public TodoItem(string text)
        {
            this.text = text;
            this.isDone = false;
            this.lastUpdate = DateTime.Now;
        }
        public string Text
        {
            get { return text; }
        }
        public bool IsDone
        {
            get { return isDone; }
        }
        public DateTime LastUpdate
        {
            get { return lastUpdate; }
        }
        public void MarkDone()
        {
            isDone = true;
            lastUpdate = DateTime.Now;
        }
        public void UpdateText(string newText)
        {
            text = newText;
            lastUpdate = DateTime.Now;
        }
        public string GetShortInfo()
        {
            string shortText = text.Length > 30 ? text.Substring(0, 27) + "..." : text;
            string status = isDone ? "Сделано" : "Не сделано";
            return $"{shortText} ({status}, обновлено {lastUpdate:dd.MM.yyyy HH:mm})";
        }
        public string GetFullInfo()
        {
            string status = isDone ? "Сделано" : "Не сделано";
            return $"Текст задачи:\n{text}\nСтатус: {status}\nДата последнего изменения: {lastUpdate:dd.MM.yyyy HH:mm}";
        }
        internal void SetLoadedState(bool done, DateTime updateDate)
        {
            isDone = done;
            lastUpdate = updateDate;
        }
    }
}