using System;
namespace TodoList.Commands
{
    internal  class CommandHelp : ICommand
    {
		public void Execute()
		{
			Console.WriteLine(@"
Доступные команды:
add <текст>          - добавить задачу
add -m / --multiline - многострочный ввод (!end - завершить)
status <номер> <статус> - изменить статус задачи
update <номер> <текст> - изменить текст задачи
delete <номер>       - удалить задачу
view [флаги]         - показать задачи
profile              - показать профиль пользователя
help                 - показать список команд
exit                 - выход
undo                 - отменить последнее действие

Доступные статусы для команды status:
NotStarted, InProgress, Completed, Postponed, Failed

Флаги для view:
-i, --index          - показывать индекс задачи
-s, --status         - показывать статус
-d, --update-date    - показывать дату изменения
-a, --all            - показывать всё
");
		}
		public void Unexecute()
		{
			
		}
	}
}
