using System;

namespace TodoList.Commands
{
    internal class AddCommand : ICommand
    {
        public TodoList List { get; set; }
        public string TaskText { get; set; }
        public AddCommand(TodoList list, string taskText)
        {
            List = list;
            TaskText = taskText;
        }
        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(TaskText))
            {
                Console.WriteLine("Текст задачи не может быть пустым.");
                return;
            }
            List.Add(new TodoItem(TaskText));
            Console.WriteLine("Задача добавлена!");
        }
    }
}

