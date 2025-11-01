using System;
namespace TodoList.Commands
{
    internal class DoneCommand : ICommand
    {
        public TodoList List { get; set; }
        public int Index { get; set; }
        public DoneCommand(TodoList list, int index)
        {
            List = list;
            Index = index;
        }
        public void Execute()
        {
            var item = List.GetItem(Index);
            if (item != null)
            {
                item.MarkDone();
                Console.WriteLine($"Задача {Index} отмечена как выполненная.");
            }
            else
            {
                Console.WriteLine("Задача с таким номером не найдена.");
            }
        }
    }
}
