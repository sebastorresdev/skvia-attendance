using FluentAssertions;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Domain.Tests;

public class ValueObjectTests
{
    [Theory]
    [InlineData("user@example.com", "user@example.com")]
    [InlineData("  test.name+alias@domain.co.uk  ", "test.name+alias@domain.co.uk")]
    public void Email_Create_WhenValidEmail_ShouldReturnEmailValueObject(string input, string expected)
    {
        // Act
        var email = Email.Create(input);

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Email_Create_WhenInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        Action act = () => Email.Create(invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("+51 987654321", "+51 987654321")]
    [InlineData("123-456-7890", "123-456-7890")]
    public void Phone_Create_WhenValidPhone_ShouldReturnPhoneValueObject(string input, string expected)
    {
        // Act
        var phone = Phone.Create(input);

        // Assert
        phone.Should().NotBeNull();
        phone.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    public void Phone_Create_WhenInvalidPhone_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act
        Action act = () => Phone.Create(invalidPhone);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}

