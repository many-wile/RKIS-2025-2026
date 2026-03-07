using System;
using Xunit;
using TodoList;
namespace TodoList.Tests;
public class TodoListTests
{
	[Fact]
	public void Add_ValidItem_IncreasesCount()
	{
		var list = new global::TodoList.TodoList();
		var item = new TodoItem("Тест");
		list.Add(item);
		Assert.Equal(1, list.Count);
		Assert.True(list.Contains(item));
	}
	[Fact]
	public void Delete_ValidIndex_RemovesItem()
	{
		var list = new global::TodoList.TodoList();
		var item = new TodoItem("Тест");
		list.Add(item);
		int targetIndex = 1;
		list.Delete(targetIndex);
		Assert.Equal(0, list.Count);
	}
	[Fact]
	public void GetItem_ValidIndex_ReturnsItem()
	{
		var list = new global::TodoList.TodoList();
		string expectedText = "Искомая задача";
		var item = new TodoItem(expectedText);
		list.Add(item);
		var result = list.GetItem(1);
		Assert.NotNull(result);
		Assert.Equal(expectedText, result.Text);
	}
	[Fact]
	public void GetItem_InvalidIndex_ReturnsNull()
	{
		var list = new global::TodoList.TodoList();
		int invalidIndexMax = 1;
		int invalidIndexMin = 0;
		var result1 = list.GetItem(invalidIndexMax);
		var result2 = list.GetItem(invalidIndexMin);
		Assert.Null(result1);
		Assert.Null(result2);
	}
	[Fact]
	public void Indexer_OutOfRange_ThrowsIndexOutOfRangeException()
	{
		var list = new global::TodoList.TodoList();
		Action act;
		act = () => { var temp = list[0]; };
		Assert.Throws<IndexOutOfRangeException>(act);
	}
}