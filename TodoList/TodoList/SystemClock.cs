namespace TodoList;

public sealed class SystemClock : IClock
{
	public DateTime Now => DateTime.Now;
}
