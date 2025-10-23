using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TodoList
    {
        internal class TodoItem
        {
            private string Text;
            private bool IsDone;
            private DateTime LastUpdate; 
            public TodoItem(string text)
            {
                this.Text = text;
                this.IsDone = false;
                this.LastUpdate = DateTime.Now;
            }
            public void MarkDone()
            {
                IsDone = true;
                LastUpdate = DateTime.Now;
            }
            public void UpdateText(string newText)
            {
                Text = newText;
                LastUpdate = DateTime.Now;
            }
            public string GetText()
            {
                return Text;
            }
            public bool GetIsDone()
            {
                return IsDone;
            }
            public DateTime GetLastUpdate()
            {
                return LastUpdate;
            }
        public string GetShortInfo()
            {
                string shortText = Text.Length > 30 ? Text.Substring(0, 27) + "..." : Text;
                string status = IsDone ? "Сделано" : "Не сделано";
                return $"{shortText} ({status}, обновлено {LastUpdate:dd.MM.yyyy HH:mm})";
            }
            public string GetFullInfo()
            {
                string status = IsDone ? "Сделано" : "Не сделано";
                return $"Текст задачи:\n{Text}\nСтатус: {status}\nДата последнего изменения: {LastUpdate:dd.MM.yyyy HH:mm}";
            }
        }
    }
