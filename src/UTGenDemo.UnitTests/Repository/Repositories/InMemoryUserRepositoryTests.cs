using Xunit;
using Shouldly;
using UTGenDemo.Repository.Repositories;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.UnitTests.Repository.Repositories;

public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _sut;

    public InMemoryUserRepositoryTests()
    {
        _sut = new InMemoryUserRepository();
    }

    [Fact]
    public void Constructor_InitializesWithSeedData()
    {
        // Arrange & Act
        var repository = new InMemoryUserRepository();

        // Assert
        repository.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUserId_ReturnsUser()
    {
        // Arrange
        var userId = "1"; // Seed data contains user with ID "1"

        // Act
        var result = await _sut.GetUserByIdAsync(userId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.Email.ShouldBe("john.doe@example.com");
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentUserId_ReturnsNull()
    {
        // Arrange
        var userId = "999";

        // Act
        var result = await _sut.GetUserByIdAsync(userId);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNullUserId_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByIdAsync(null!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithEmptyUserId_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByIdAsync("");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var email = "john.doe@example.com"; // Seed data contains this email

        // Act
        var result = await _sut.GetUserByEmailAsync(email);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(email);
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmailDifferentCase_ReturnsUser()
    {
        // Arrange
        var email = "JOHN.DOE@EXAMPLE.COM"; // Different case

        // Act
        var result = await _sut.GetUserByEmailAsync(email);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe("john.doe@example.com"); // Original case
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _sut.GetUserByEmailAsync(email);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNullEmail_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByEmailAsync(null!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithEmptyEmail_ReturnsNull()
    {
        // Arrange & Act
        var result = await _sut.GetUserByEmailAsync("");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetActiveUsersAsync_ReturnsOnlyActiveUsers()
    {
        // Act
        var result = await _sut.GetActiveUsersAsync();

        // Assert
        var activeUsers = result.ToList();
        activeUsers.Count.ShouldBe(2); // Based on seed data: John and Jane are active
        activeUsers.ShouldAllBe(u => u.IsActive);
        activeUsers.ShouldContain(u => u.Email == "john.doe@example.com");
        activeUsers.ShouldContain(u => u.Email == "jane.smith@example.com");
        activeUsers.ShouldNotContain(u => u.Email == "inactive.user@example.com");
    }

    [Fact]
    public async Task CreateUserAsync_WithValidUser_CreatesUserWithGeneratedIdAndTimestamp()
    {
        // Arrange
        var user = new User
        {
            Email = "new.user@example.com",
            FirstName = "New",
            LastName = "User",
            IsActive = true
        };
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await _sut.CreateUserAsync(user);

        // Assert
        result.ShouldBe(user);
        user.Id.ShouldNotBeNullOrWhiteSpace();
        user.Id.ShouldNotBe("0");
        user.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeCreate);
        user.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);

        // Verify user is actually added to repository
        var retrievedUser = await _sut.GetUserByEmailAsync(user.Email);
        retrievedUser.ShouldBe(user);
    }

    [Fact]
    public async Task CreateUserAsync_MultipleUsers_GeneratesUniqueIds()
    {
        // Arrange
        var user1 = new User { Email = "user1@example.com", FirstName = "User", LastName = "One" };
        var user2 = new User { Email = "user2@example.com", FirstName = "User", LastName = "Two" };

        // Act
        var result1 = await _sut.CreateUserAsync(user1);
        var result2 = await _sut.CreateUserAsync(user2);

        // Assert
        result1.Id.ShouldNotBe(result2.Id);
        result1.Id.ShouldNotBeNullOrWhiteSpace();
        result2.Id.ShouldNotBeNullOrWhiteSpace();
        int.Parse(result2.Id).ShouldBeGreaterThan(int.Parse(result1.Id));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_UpdatesUserProperties()
    {
        // Arrange
        var existingUserId = "1"; // From seed data
        var originalUser = await _sut.GetUserByIdAsync(existingUserId);
        var updatedUser = new User
        {
            Id = existingUserId,
            Email = "updated.email@example.com",
            FirstName = "Updated",
            LastName = "Name",
            IsActive = false
        };

        // Act
        var result = await _sut.UpdateUserAsync(updatedUser);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingUserId);
        result.Email.ShouldBe("updated.email@example.com");
        result.FirstName.ShouldBe("Updated");
        result.LastName.ShouldBe("Name");
        result.IsActive.ShouldBeFalse();

        // Verify the original user reference was updated
        originalUser!.Email.ShouldBe("updated.email@example.com");
        originalUser.FirstName.ShouldBe("Updated");
        originalUser.LastName.ShouldBe("Name");
        originalUser.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentUser = new User
        {
            Id = "999",
            Email = "nonexistent@example.com",
            FirstName = "Non",
            LastName = "Existent"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _sut.UpdateUserAsync(nonExistentUser));
        exception.Message.ShouldContain("User with ID 999 not found");
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUserId_DeletesUserAndReturnsTrue()
    {
        // Arrange
        var userId = "2"; // From seed data (Jane Smith)

        // Verify user exists before deletion
        var userBeforeDelete = await _sut.GetUserByIdAsync(userId);
        userBeforeDelete.ShouldNotBeNull();

        // Act
        var result = await _sut.DeleteUserAsync(userId);

        // Assert
        result.ShouldBeTrue();

        // Verify user no longer exists
        var userAfterDelete = await _sut.GetUserByIdAsync(userId);
        userAfterDelete.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUserId_ReturnsFalse()
    {
        // Arrange
        var nonExistentUserId = "999";

        // Act
        var result = await _sut.DeleteUserAsync(nonExistentUserId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_WithNullUserId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.DeleteUserAsync(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_WithEmptyUserId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.DeleteUserAsync("");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUserId_ReturnsTrue()
    {
        // Arrange
        var existingUserId = "1"; // From seed data

        // Act
        var result = await _sut.ExistsAsync(existingUserId);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentUserId_ReturnsFalse()
    {
        // Arrange
        var nonExistentUserId = "999";

        // Act
        var result = await _sut.ExistsAsync(nonExistentUserId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithNullUserId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.ExistsAsync(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithEmptyUserId_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.ExistsAsync("");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task Repository_MaintainsDataConsistency_AfterMultipleOperations()
    {
        // Arrange
        var newUser = new User { Email = "test@example.com", FirstName = "Test", LastName = "User" };

        // Act & Assert - Create
        var createdUser = await _sut.CreateUserAsync(newUser);
        createdUser.ShouldNotBeNull();
        var exists = await _sut.ExistsAsync(createdUser.Id);
        exists.ShouldBeTrue();

        // Act & Assert - Update
        createdUser.FirstName = "Updated";
        var updatedUser = await _sut.UpdateUserAsync(createdUser);
        updatedUser.FirstName.ShouldBe("Updated");

        // Act & Assert - Retrieve
        var retrievedUser = await _sut.GetUserByIdAsync(createdUser.Id);
        retrievedUser!.FirstName.ShouldBe("Updated");

        // Act & Assert - Delete
        var deleted = await _sut.DeleteUserAsync(createdUser.Id);
        deleted.ShouldBeTrue();

        // Act & Assert - Verify deletion
        var deletedUser = await _sut.GetUserByIdAsync(createdUser.Id);
        deletedUser.ShouldBeNull();
        var existsAfterDelete = await _sut.ExistsAsync(createdUser.Id);
        existsAfterDelete.ShouldBeFalse();
    }

    [Fact]
    public async Task SeedData_ContainsExpectedUsers()
    {
        // This test verifies the seed data is correctly initialized
        
        // Act
        var john = await _sut.GetUserByEmailAsync("john.doe@example.com");
        var jane = await _sut.GetUserByEmailAsync("jane.smith@example.com");
        var inactive = await _sut.GetUserByEmailAsync("inactive.user@example.com");

        // Assert
        john.ShouldNotBeNull();
        john.Id.ShouldBe("1");
        john.FirstName.ShouldBe("John");
        john.LastName.ShouldBe("Doe");
        john.IsActive.ShouldBeTrue();

        jane.ShouldNotBeNull();
        jane.Id.ShouldBe("2");
        jane.FirstName.ShouldBe("Jane");
        jane.LastName.ShouldBe("Smith");
        jane.IsActive.ShouldBeTrue();

        inactive.ShouldNotBeNull();
        inactive.Id.ShouldBe("3");
        inactive.FirstName.ShouldBe("Inactive");
        inactive.LastName.ShouldBe("User");
        inactive.IsActive.ShouldBeFalse();
    }
}