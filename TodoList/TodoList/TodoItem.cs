using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
    {
        internal class TodoItem
        {
            public string Text;
            public bool IsDone;
            public DateTime LastUpdate; 
            public TodoItem(string text)
            {
                Text = text;
                IsDone = false;
                LastUpdate = DateTime.Now;
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

}
}
