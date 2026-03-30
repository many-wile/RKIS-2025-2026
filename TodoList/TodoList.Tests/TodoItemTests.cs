using Moq;
using TodoList;

namespace TodoList.Tests;

public class TodoItemTests
{
	[Fact]
	public void Constructor_ClockProvided_SetsLastUpdateFromClock()
	{
		// Arrange
		string text = "Купить хлеб";
		DateTime fixedTime = new DateTime(2025, 1, 1, 12, 0, 0);
		var clockMock = new Mock<IClock>();
		clockMock.Setup(c => c.Now).Returns(fixedTime);

		// Act
		TodoItem item = new TodoItem(text, clockMock.Object);

		// Assert
		Assert.Equal(text, item.Text);
		Assert.Equal(TodoStatus.NotStarted, item.Status);
		Assert.Equal(fixedTime, item.LastUpdate);
	}

	[Fact]
	public void ChangeStatus_ClockProvided_UpdatesStatusAndLastUpdate()
	{
		// Arrange
		DateTime oldTime = new DateTime(2024, 12, 31, 10, 0, 0);
		DateTime changedTime = new DateTime(2025, 1, 2, 9, 30, 0);
		var clockMock = new Mock<IClock>();
		clockMock.Setup(c => c.Now).Returns(changedTime);
		TodoItem item = new TodoItem("Тест", TodoStatus.NotStarted, oldTime, clockMock.Object);

		// Act
		item.ChangeStatus(TodoStatus.Completed);

		// Assert
		Assert.Equal(TodoStatus.Completed, item.Status);
		Assert.Equal(changedTime, item.LastUpdate);
	}

	[Fact]
	public void UpdateText_ClockProvided_UpdatesTextAndLastUpdate()
	{
		// Arrange
		DateTime oldTime = new DateTime(2024, 12, 31, 8, 0, 0);
		DateTime changedTime = new DateTime(2025, 1, 3, 14, 15, 0);
		var clockMock = new Mock<IClock>();
		clockMock.Setup(c => c.Now).Returns(changedTime);
		TodoItem item = new TodoItem("Старый текст", TodoStatus.NotStarted, oldTime, clockMock.Object);

		// Act
		item.UpdateText("Новый текст");

		// Assert
		Assert.Equal("Новый текст", item.Text);
		Assert.Equal(changedTime, item.LastUpdate);
	}
}
