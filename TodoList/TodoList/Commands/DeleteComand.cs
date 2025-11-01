namespace TodoList.Commands
{
    internal class DeleteCommand : ICommand
    {
        private TodoList todoList;
        private int index;
        public DeleteCommand(TodoList todoList, int index)
        {
            this.todoList = todoList;
            this.index = index;
        }
        public void Execute()
        {
            todoList.Delete(index);
        }
    }
}
