using System;
using Xunit;
using TodoList;
using TodoList.Commands;
using TodoList.Exceptions;
namespace TodoList.Tests;
public class CommandParserTests
{
	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("   ")]
	[InlineData("\t")]
	public void Parse_EmptyOrWhitespaceString_ReturnsNull(string input)
	{
		var result = CommandParser.Parse(input);
		Assert.Null(result);
	}
	[Theory]
	[InlineData("add купить хлеб", "купить хлеб")]
	[InlineData("add    помыть окно   ", "помыть окно")] 
	[InlineData("Add сходить в кино", "сходить в кино")] 
	public void Parse_ValidAddCommand_ReturnsAddCommandWithCorrectText(string input, string expectedText)
	{
		var result = CommandParser.Parse(input);
		var addCommand = Assert.IsType<AddCommand>(result);
		Assert.Equal(expectedText, addCommand.TaskText);
	}
	[Theory]
	[InlineData("fly to the moon")]
	[InlineData("создать задачу")]
	[InlineData("12345")]
	[InlineData("ad d test")]
	public void Parse_UnknownCommand_ThrowsInvalidCommandException(string input)
	{
		Action act;
		act = () => CommandParser.Parse(input);
		var exception = Assert.Throws<InvalidCommandException>(act);
		Assert.Contains("не найдена", exception.Message);
	}
	[Theory]
	[InlineData("add")]
	[InlineData("add ")]
	[InlineData("add    ")]
	public void Parse_AddWithoutText_ThrowsInvalidArgumentException(string input)
	{
		Action act;
		act = () => CommandParser.Parse(input);
		Assert.Throws<InvalidArgumentException>(act);
	}
	[Theory]
	[InlineData("delete")]
	[InlineData("delete abc")]
	[InlineData("delete 1.5")]
	[InlineData("delete номер 5")]
	public void Parse_DeleteWithInvalidFormat_ThrowsInvalidArgumentException(string input)
	{
		Action act;
		act = () => CommandParser.Parse(input);
		Assert.Throws<InvalidArgumentException>(act);
	}
	[Theory]
	[InlineData("status 1 SuperDone")]
	[InlineData("status abc Completed")]
	[InlineData("status")]
	[InlineData("status 1")]
	public void Parse_StatusWithInvalidArguments_ThrowsInvalidArgumentException(string input)
	{
		Action act;
		act = () => CommandParser.Parse(input);
		Assert.Throws<InvalidArgumentException>(act);
	}
}