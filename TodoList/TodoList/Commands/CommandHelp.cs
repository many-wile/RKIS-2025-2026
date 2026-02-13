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
add -m               - многострочный ввод
delete <номер>       - удалить задачу
update <номер> <txt> - изменить текст задачи
status <номер> <st>  - изменить статус задачи
view [флаги]         - показать задачи
undo / redo          - отмена / повтор действия
profile (-o)         - профиль (выход)

ПОИСКОВАЯ СИСТЕМА aka ЛОКАТОР (search):
search <текст>       - простой поиск
search [флаги]       - расширенный поиск

Флаги поиска:
--contains ""txt""     - содержит текст
--starts-with ""txt""  - начинается с текста
--ends-with ""txt""    - заканчивается текстом
--status <status>    - фильтр по статусу
--from <yyyy-MM-dd>  - дата изменения НЕ раньше
--to <yyyy-MM-dd>    - дата изменения НЕ позже
--sort <text|date>   - сортировка
--desc               - по убыванию
--top <n>            - показать топ-N результатов

Пример: 
search --status InProgress --sort date --desc --top 5
");
		}
	}
}