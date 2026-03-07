using System;
using Xunit;
using TodoList;
namespace TodoList.Tests;
public class ProfileTests
{
	[Fact]
	public void Constructor_SetsPropertiesCorrectly()
	{
		string firstName = "Иван";
		string lastName = "Иванов";
		int birthYear = 1990;
		string login = "ivan90";
		string password = "password123";
		Profile profile = new Profile(firstName, lastName, birthYear, login, password);
		Assert.NotEqual(Guid.Empty, profile.Id);
		Assert.Equal(firstName, profile.FirstName);
		Assert.Equal(lastName, profile.LastName);
		Assert.Equal(birthYear, profile.BirthYear);
		Assert.Equal(login, profile.Login);
		Assert.Equal(password, profile.Password);
	}
	[Fact]
	public void ToCsvString_ReturnsCorrectFormat()
	{
		Guid expectedId = Guid.NewGuid();
		Profile profile = new Profile(expectedId, "user", "pass", "Name", "Last", 2000);
		string expectedCsv = $"{expectedId};user;pass;Name;Last;2000";
		string actualCsv = profile.ToCsvString();
		Assert.Equal(expectedCsv, actualCsv);
	}
	[Fact]
	public void GetInfo_CalculatesAgeCorrectly()
	{
		int currentYear = DateTime.Now.Year;
		int birthYear = 1990;
		int expectedAge = currentYear - birthYear;
		Profile profile = new Profile("Иван", "Иванов", birthYear, "login", "pass");
		string info = profile.GetInfo();
		Assert.Contains(expectedAge.ToString(), info);
		Assert.Contains("Иван Иванов", info);
		Assert.Contains("login", info);
	}
}