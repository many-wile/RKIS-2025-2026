using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TodoList
{
    internal class UpdateCommand : ICommand
    {
        public TodoList List { get; set; }
        public int Index { get; set; }
        public string NewText { get; set; }
        public UpdateCommand(TodoList list, int index, string newText)
        {
            List = list;
            Index = index;
            NewText = newText;
        }
        public void Execute()
        {
            var item = List.GetItem(Index);
            if (item != null)
            {
                item.UpdateText(NewText);
                Console.WriteLine($"Задача {Index} обновлена.");
            }
            else
            {
                Console.WriteLine("Задача с таким номером не найдена.");
            }
        }
    }
}
