using Xunit;
using Shouldly;
using UTGenDemo.Service.Services;

namespace UTGenDemo.UnitTests.Service.Services;

public class MockEmailServiceTests
{
    private readonly MockEmailService _sut;

    public MockEmailServiceTests()
    {
        _sut = new MockEmailService();
    }

    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Arrange & Act
        var service = new MockEmailService();

        // Assert
        service.ShouldNotBeNull();
    }

    [Fact]
    public async Task SendEmailAsync_WithValidParameters_ReturnsTrue()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var result = await _sut.SendEmailAsync(to, subject, body);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithNullTo_ReturnsTrue()
    {
        // Arrange
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var result = await _sut.SendEmailAsync(null!, subject, body);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithEmptyTo_ReturnsTrue()
    {
        // Arrange
        var to = "";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var result = await _sut.SendEmailAsync(to, subject, body);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithNullSubject_ReturnsTrue()
    {
        // Arrange
        var to = "test@example.com";
        var body = "Test Body";

        // Act
        var result = await _sut.SendEmailAsync(to, null!, body);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithNullBody_ReturnsTrue()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";

        // Act
        var result = await _sut.SendEmailAsync(to, subject, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithAllNullParameters_ReturnsTrue()
    {
        // Arrange & Act
        var result = await _sut.SendEmailAsync(null!, null!, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithValidParameters_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var userName = "John Doe";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(userEmail, userName);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithNullUserEmail_ReturnsTrue()
    {
        // Arrange
        var userName = "John Doe";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(null!, userName);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithEmptyUserEmail_ReturnsTrue()
    {
        // Arrange
        var userEmail = "";
        var userName = "John Doe";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(userEmail, userName);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithNullUserName_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(userEmail, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithEmptyUserName_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var userName = "";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(userEmail, userName);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithAllNullParameters_ReturnsTrue()
    {
        // Arrange & Act
        var result = await _sut.SendWelcomeEmailAsync(null!, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithValidParameters_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var resetToken = "reset-token-123";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(userEmail, resetToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithNullUserEmail_ReturnsTrue()
    {
        // Arrange
        var resetToken = "reset-token-123";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(null!, resetToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithEmptyUserEmail_ReturnsTrue()
    {
        // Arrange
        var userEmail = "";
        var resetToken = "reset-token-123";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(userEmail, resetToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithNullResetToken_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(userEmail, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithEmptyResetToken_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var resetToken = "";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(userEmail, resetToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithAllNullParameters_ReturnsTrue()
    {
        // Arrange & Act
        var result = await _sut.SendPasswordResetEmailAsync(null!, null!);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_WithSpecialCharactersInToken_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var resetToken = "token-with-!@#$%^&*()_+-={}[]|\\:;\"'<>?,./`~";

        // Act
        var result = await _sut.SendPasswordResetEmailAsync(userEmail, resetToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithSpecialCharactersInUserName_ReturnsTrue()
    {
        // Arrange
        var userEmail = "user@example.com";
        var userName = "José María O'Connor-Smith";

        // Act
        var result = await _sut.SendWelcomeEmailAsync(userEmail, userName);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithVeryLongContent_ReturnsTrue()
    {
        // Arrange
        var to = "test@example.com";
        var subject = new string('A', 1000); // Very long subject
        var body = new string('B', 10000); // Very long body

        // Act
        var result = await _sut.SendEmailAsync(to, subject, body);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task AllEmailMethods_AreAsync()
    {
        // This test ensures all email methods are properly async and return Task<bool>
        
        // Arrange
        var tasks = new List<Task<bool>>
        {
            _sut.SendEmailAsync("test@example.com", "Subject", "Body"),
            _sut.SendWelcomeEmailAsync("test@example.com", "User Name"),
            _sut.SendPasswordResetEmailAsync("test@example.com", "token")
        };

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        results.ShouldAllBe(result => result == true);
    }
}