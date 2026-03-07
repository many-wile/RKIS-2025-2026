using System;
using Xunit;
using TodoList;
namespace TodoList.Tests;
public class TodoItemTests
{
	[Fact]
	public void NewTodoItem_HasNotStartedStatus_AndCurrentDate()
	{
		string text = "Купить хлеб";
		TodoItem item = new TodoItem(text);
		Assert.Equal(text, item.Text);
		Assert.Equal(TodoStatus.NotStarted, item.Status);
		Assert.True((DateTime.Now - item.LastUpdate).TotalSeconds < 1);
	}
	[Fact]
	public void ChangeStatus_UpdatesStatus_AndChangesLastUpdate()
	{
		TodoItem item = new TodoItem("Тест", TodoStatus.NotStarted, DateTime.Now.AddDays(-1));
		DateTime oldDate = item.LastUpdate;
		item.ChangeStatus(TodoStatus.Completed);
		Assert.Equal(TodoStatus.Completed, item.Status);
		Assert.NotEqual(oldDate, item.LastUpdate);
	}
	[Fact]
	public void UpdateText_ChangesText_AndChangesLastUpdate()
	{
		TodoItem item = new TodoItem("Старый текст", TodoStatus.NotStarted, DateTime.Now.AddDays(-1));
		item.UpdateText("Новый текст");
		Assert.Equal("Новый текст", item.Text);
		Assert.True(item.LastUpdate.Date == DateTime.Now.Date);
	}
}