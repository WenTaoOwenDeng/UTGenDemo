using Xunit;
using Shouldly;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.UnitTests.Repository.Models;

public class UserTests
{
    [Fact]
    public void Constructor_CreatesUserWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.ShouldBe(string.Empty);
        user.Email.ShouldBe(string.Empty);
        user.FirstName.ShouldBe(string.Empty);
        user.LastName.ShouldBe(string.Empty);
        user.CreatedAt.ShouldBe(default(DateTime));
        user.IsActive.ShouldBeTrue(); // Default value is true
    }

    [Fact]
    public void FullName_WithBothFirstAndLastName_ReturnsCombinedName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe("John Doe");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ReturnsFirstNameTrimmed()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = ""
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe("John");
    }

    [Fact]
    public void FullName_WithOnlyLastName_ReturnsLastNameTrimmed()
    {
        // Arrange
        var user = new User
        {
            FirstName = "",
            LastName = "Doe"
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe("Doe");
    }

    [Fact]
    public void FullName_WithBothNamesEmpty_ReturnsEmptyString()
    {
        // Arrange
        var user = new User
        {
            FirstName = "",
            LastName = ""
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe(string.Empty);
    }

    [Fact]
    public void FullName_WithWhitespaceNames_ReturnsEmptyStringTrimmed()
    {
        // Arrange
        var user = new User
        {
            FirstName = "   ",
            LastName = "   "
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe(string.Empty);
    }

    [Fact]
    public void FullName_WithExtraSpaces_ReturnsProperlyFormattedName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "  John  ",
            LastName = "  Doe  "
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.ShouldBe("John     Doe"); // The actual behavior: extra spaces are preserved between names
    }

    [Fact]
    public void IsEmailValid_WithValidEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User { Email = "john.doe@example.com" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsEmailValid_WithSimpleValidEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User { Email = "test@domain.com" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsEmailValid_WithComplexValidEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User { Email = "user.name+tag@sub.domain.org" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsEmailValid_WithNullEmail_ReturnsFalse()
    {
        // Arrange
        var user = new User { Email = null! };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsEmailValid_WithEmptyEmail_ReturnsFalse()
    {
        // Arrange
        var user = new User { Email = "" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsEmailValid_WithWhitespaceEmail_ReturnsFalse()
    {
        // Arrange
        var user = new User { Email = "   " };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsEmailValid_WithEmailWithoutAtSymbol_ReturnsFalse()
    {
        // Arrange
        var user = new User { Email = "invalidemail.com" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsEmailValid_WithEmailWithOnlyAtSymbol_ReturnsTrue()
    {
        // Note: This is a simple validation - just checks for @ presence
        // Arrange
        var user = new User { Email = "@" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue(); // Based on the simple validation logic in the code
    }

    [Fact]
    public void IsEmailValid_WithEmailWithMultipleAtSymbols_ReturnsTrue()
    {
        // Note: This is a simple validation - just checks for @ presence
        // Arrange
        var user = new User { Email = "user@@domain.com" };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue(); // Based on the simple validation logic in the code
    }

    [Theory]
    [InlineData("user@domain.com", "user@domain.com")]
    [InlineData("test@example.org", "test@example.org")]
    [InlineData("name.surname@company.co.uk", "name.surname@company.co.uk")]
    public void IsEmailValid_WithVariousValidEmails_ReturnsTrue(string email, string expectedEmail)
    {
        // Arrange
        var user = new User { Email = email };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeTrue();
        user.Email.ShouldBe(expectedEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("invalid.email")]
    [InlineData("invalid.email.com")]
    public void IsEmailValid_WithVariousInvalidEmails_ReturnsFalse(string email)
    {
        // Arrange
        var user = new User { Email = email };

        // Act
        var isValid = user.IsEmailValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void User_AllPropertiesCanBeSetAndRetrieved()
    {
        // Arrange
        var id = "123";
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var createdAt = DateTime.UtcNow;
        var isActive = false;

        // Act
        var user = new User
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = createdAt,
            IsActive = isActive
        };

        // Assert
        user.Id.ShouldBe(id);
        user.Email.ShouldBe(email);
        user.FirstName.ShouldBe(firstName);
        user.LastName.ShouldBe(lastName);
        user.CreatedAt.ShouldBe(createdAt);
        user.IsActive.ShouldBe(isActive);
        user.FullName.ShouldBe("John Doe");
        user.IsEmailValid().ShouldBeTrue();
    }

    [Fact]
    public void User_WithSpecialCharactersInNames_HandlesCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "José María",
            LastName = "O'Connor-Smith"
        };

        // Assert
        user.FullName.ShouldBe("José María O'Connor-Smith");
    }

    [Fact]
    public void User_IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void User_CanBeDeactivated()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.ShouldBeFalse();
    }
}