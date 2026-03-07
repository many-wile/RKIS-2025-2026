using System;
using Xunit;
using TodoList;
namespace TodoList.Tests;
public class TodoItemTests
{
	[Fact]
	public void Constructor_ValidText_SetsDefaultStatusAndDate()
	{
		string text = "Купить хлеб";
		TodoItem item = new TodoItem(text);
		Assert.Equal(text, item.Text);
		Assert.Equal(TodoStatus.NotStarted, item.Status);
		Assert.True((DateTime.Now - item.LastUpdate).TotalSeconds < 1);
	}
	[Fact]
	public void ChangeStatus_ValidStatus_UpdatesStatusAndLastUpdate()
	{
		TodoItem item = new TodoItem("Тест", TodoStatus.NotStarted, DateTime.Now.AddDays(-1));
		DateTime oldDate = item.LastUpdate;
		TodoStatus newStatus = TodoStatus.Completed;
		item.ChangeStatus(newStatus);
		Assert.Equal(newStatus, item.Status);
		Assert.NotEqual(oldDate, item.LastUpdate);
	}
	[Fact]
	public void UpdateText_ValidText_ChangesTextAndLastUpdate()
	{
		TodoItem item = new TodoItem("Старый текст", TodoStatus.NotStarted, DateTime.Now.AddDays(-1));
		string newText = "Новый текст";
		item.UpdateText(newText);
		Assert.Equal(newText, item.Text);
		Assert.True(item.LastUpdate.Date == DateTime.Now.Date);
	}
}