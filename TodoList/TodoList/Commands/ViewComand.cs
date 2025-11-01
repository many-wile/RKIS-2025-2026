using System;
namespace TodoList.Commands
{
    internal class ViewCommand : ICommand
    {
        public TodoList List { get; set; }
        public bool ShowIndex { get; set; }
        public bool ShowDone { get; set; }
        public bool ShowDate { get; set; }
        public bool ShowAll { get; set; }
        public ViewCommand(TodoList list)
        {
            List = list;
        }
        public void Execute()
        {
            if (ShowAll)
            {
                ShowIndex = ShowDone = ShowDate = true;
            }
            List.View(ShowIndex, ShowDone, ShowDate);
        }
    }
}
