using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Domain.Tests;

public class ValueObjectTests
{
    [Theory]
    [InlineData("user@example.com", "user@example.com")]
    [InlineData("  test.name+alias@domain.co.uk  ", "test.name+alias@domain.co.uk")]
    public void Email_Create_ValidEmail_ReturnsEmailValueObject(string input, string expected)
    {
        var email = Email.Create(input);
        Assert.Equal(expected, email.Value);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Email_Create_InvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Theory]
    [InlineData("+51 987654321", "+51 987654321")]
    [InlineData("123-456-7890", "123-456-7890")]
    public void Phone_Create_ValidPhone_ReturnsPhoneValueObject(string input, string expected)
    {
        var phone = Phone.Create(input);
        Assert.Equal(expected, phone.Value);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    public void Phone_Create_InvalidPhone_ThrowsArgumentException(string invalidPhone)
    {
        Assert.Throws<ArgumentException>(() => Phone.Create(invalidPhone));
    }
}
