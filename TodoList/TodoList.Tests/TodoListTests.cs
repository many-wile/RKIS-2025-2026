using System;
using Xunit;
using TodoList;
namespace TodoList.Tests;
public class TodoListTests
{
	[Fact]
	public void Add_IncreasesCount()
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
		list.Delete(1);
		Assert.Equal(0, list.Count);
	}
	[Fact]
	public void GetItem_ValidIndex_ReturnsItem()
	{
		var list = new global::TodoList.TodoList();
		var item = new TodoItem("Искомая задача");
		list.Add(item);
		var result = list.GetItem(1);
		Assert.NotNull(result);
		Assert.Equal("Искомая задача", result.Text);
	}
	[Fact]
	public void GetItem_InvalidIndex_ReturnsNull()
	{
		var list = new global::TodoList.TodoList();
		var result1 = list.GetItem(1);
		var result2 = list.GetItem(0);
		Assert.Null(result1);
		Assert.Null(result2);
	}
	[Fact]
	public void Indexer_OutOfRange_ThrowsIndexOutOfRangeException()
	{
		var list = new global::TodoList.TodoList();
		Assert.Throws<IndexOutOfRangeException>(() => { var temp = list[0]; });
	}
}