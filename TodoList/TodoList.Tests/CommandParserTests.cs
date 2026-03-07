using System;
using Xunit;
using TodoList;
using TodoList.Commands;
using TodoList.Exceptions;
namespace TodoList.Tests;
public class CommandParserTests
{
	[Fact]
	public void Parse_EmptyString_ReturnsNull()
	{
		var result = CommandParser.Parse("   ");
		Assert.Null(result);
	}
	[Fact]
	public void Parse_ValidAddCommand_ReturnsAddCommandInstance()
	{
		var result = CommandParser.Parse("add Купить молоко");
		var addCommand = Assert.IsType<AddCommand>(result);
		Assert.Equal("Купить молоко", addCommand.TaskText);
	}
	[Fact]
	public void Parse_ValidDeleteCommand_ReturnsDeleteCommandInstance()
	{
		var result = CommandParser.Parse("delete 5");
		Assert.IsType<DeleteCommand>(result);
	}
	[Fact]
	public void Parse_UnknownCommand_ThrowsInvalidCommandException()
	{
		var exception = Assert.Throws<InvalidCommandException>(() =>
			CommandParser.Parse("fly to the moon")
		);
		Assert.Contains("не найдена", exception.Message);
	}
	[Fact]
	public void Parse_AddWithoutText_ThrowsInvalidArgumentException()
	{
		Assert.Throws<InvalidArgumentException>(() =>
			CommandParser.Parse("add ")
		);
	}
	[Fact]
	public void Parse_StatusWithInvalidStatusName_ThrowsInvalidArgumentException()
	{
		Assert.Throws<InvalidArgumentException>(() =>
			CommandParser.Parse("status 1 SuperDone")
		);
	}
}