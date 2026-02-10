using System;
namespace TodoList.Commands
{
	internal class CommandHelp : ICommand
	{
		public void Execute()
		{
			Console.WriteLine(@"
Доступные команды:
add <текст>          - добавить задачу
add -m / --multiline - многострочный ввод (!end - завершить)
delete <номер>       - удалить задачу
update <номер> <текст> - изменить текст задачи
status <номер> <статус> - изменить статус задачи
search <текст>       - поиск задач по содержимому
view [флаги]         - показать задачи
profile              - показать профиль пользователя
profile -o / --out   - выйти из системы (logout)
undo                 - отменить последнее действие
redo                 - повторить отмененное действие
help                 - показать список команд
exit                 - выход из программы

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