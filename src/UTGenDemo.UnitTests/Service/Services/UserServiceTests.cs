using Xunit;
using Moq;
using Shouldly;
using UTGenDemo.Service.Services;
using UTGenDemo.Service.Interfaces;
using UTGenDemo.Repository.Models;
using UTGenDemo.Repository.Interfaces;

namespace UTGenDemo.UnitTests.Service.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _sut = new UserService(_userRepositoryMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => new UserService(null!, _emailServiceMock.Object));
        exception.ParamName.ShouldBe("userRepository");
    }

    [Fact]
    public void Constructor_WithNullEmailService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => new UserService(_userRepositoryMock.Object, null!));
        exception.ParamName.ShouldBe("emailService");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new UserService(_userRepositoryMock.Object, _emailServiceMock.Object);

        // Assert
        service.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNullUserId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetUserByIdAsync(null!));
        exception.ParamName.ShouldBe("userId");
        exception.Message.ShouldContain("User ID cannot be null or empty");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetUserByIdAsync(""));
        exception.ParamName.ShouldBe("userId");
        exception.Message.ShouldContain("User ID cannot be null or empty");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithWhitespaceUserId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.GetUserByIdAsync("   "));
        exception.ParamName.ShouldBe("userId");
        exception.Message.ShouldContain("User ID cannot be null or empty");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidUserId_UserExists_ReturnsUser()
    {
        // Arrange
        var userId = "123";
        var expectedUser = new User { Id = userId, Email = "test@example.com" };
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _sut.GetUserByIdAsync(userId);

        // Assert
        result.ShouldBe(expectedUser);
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidUserId_UserNotExists_ReturnsNull()
    {
        // Arrange
        var userId = "123";
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetUserByIdAsync(userId);

        // Assert
        result.ShouldBeNull();
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNullEmail_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByEmailAsync(null!);

        // Assert
        result.ShouldBeNull();
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithEmptyEmail_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByEmailAsync("");

        // Assert
        result.ShouldBeNull();
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithWhitespaceEmail_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByEmailAsync("   ");

        // Assert
        result.ShouldBeNull();
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_UserExists_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new User { Id = "123", Email = email };
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _sut.GetUserByEmailAsync(email);

        // Assert
        result.ShouldBe(expectedUser);
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_UserNotExists_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetUserByEmailAsync(email);

        // Assert
        result.ShouldBeNull();
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentNullException>(() => _sut.CreateUserAsync(null!));
        exception.ParamName.ShouldBe("user");
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Email = "invalid-email", FirstName = "John", LastName = "Doe" };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.CreateUserAsync(user));
        exception.ParamName.ShouldBe("user");
        exception.Message.ShouldContain("Invalid email address");
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User { Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var existingUser = new User { Id = "123", Email = "test@example.com" };
        
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _sut.CreateUserAsync(user));
        exception.Message.ShouldContain($"User with email {user.Email} already exists");
    }

    [Fact]
    public async Task CreateUserAsync_WithValidUser_CreatesUserAndSendsWelcomeEmail()
    {
        // Arrange
        var user = new User { Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var createdUser = new User { Id = "123", Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);
        _emailServiceMock.Setup(e => e.SendWelcomeEmailAsync(user.Email, user.FullName))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateUserAsync(user);

        // Assert
        result.ShouldBe(createdUser);
        user.Id.ShouldNotBeNullOrWhiteSpace();
        user.CreatedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));

        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(user.Email), Times.Once);
        _userRepositoryMock.Verify(r => r.CreateUserAsync(It.Is<User>(u => 
            u.Id != null && 
            u.CreatedAt != default && 
            u.Email == user.Email)), Times.Once);
        _emailServiceMock.Verify(e => e.SendWelcomeEmailAsync(user.Email, user.FullName), Times.Once);
    }

    [Fact]
    public async Task GetActiveUsersAsync_ReturnsActiveUsers()
    {
        // Arrange
        var activeUsers = new List<User>
        {
            new User { Id = "1", IsActive = true },
            new User { Id = "2", IsActive = true }
        };
        _userRepositoryMock.Setup(r => r.GetActiveUsersAsync())
            .ReturnsAsync(activeUsers);

        // Act
        var result = await _sut.GetActiveUsersAsync();

        // Assert
        result.ShouldBe(activeUsers);
        _userRepositoryMock.Verify(r => r.GetActiveUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var userId = "123";
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.DeactivateUserAsync(userId);

        // Assert
        result.ShouldBeFalse();
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateUserAsync_WithExistingUser_DeactivatesUserAndReturnsTrue()
    {
        // Arrange
        var userId = "123";
        var user = new User { Id = userId, IsActive = true };
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.DeactivateUserAsync(userId);

        // Assert
        result.ShouldBeTrue();
        user.IsActive.ShouldBeFalse();
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = await Should.ThrowAsync<ArgumentNullException>(() => _sut.UpdateUserAsync(null!));
        exception.ParamName.ShouldBe("user");
    }

    [Fact]
    public async Task UpdateUserAsync_WithNullUserId_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Id = null!, Email = "test@example.com" };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.UpdateUserAsync(user));
        exception.ParamName.ShouldBe("user");
        exception.Message.ShouldContain("User ID is required for updates");
    }

    [Fact]
    public async Task UpdateUserAsync_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Id = "", Email = "test@example.com" };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.UpdateUserAsync(user));
        exception.ParamName.ShouldBe("user");
        exception.Message.ShouldContain("User ID is required for updates");
    }

    [Fact]
    public async Task UpdateUserAsync_WithWhitespaceUserId_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Id = "   ", Email = "test@example.com" };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _sut.UpdateUserAsync(user));
        exception.ParamName.ShouldBe("user");
        exception.Message.ShouldContain("User ID is required for updates");
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = "123", Email = "test@example.com" };
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _sut.UpdateUserAsync(user));
        exception.Message.ShouldContain($"User with ID {user.Id} not found");
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_UpdatesUserAndReturnsUpdatedUser()
    {
        // Arrange
        var user = new User { Id = "123", Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var existingUser = new User { Id = "123", Email = "old@example.com" };
        var updatedUser = new User { Id = "123", Email = "test@example.com", FirstName = "John", LastName = "Doe" };

        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id))
            .ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(r => r.UpdateUserAsync(user))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _sut.UpdateUserAsync(user);

        // Assert
        result.ShouldBe(updatedUser);
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(user.Id), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
}