# Unit Test Generator Demo Guide

## Quick Start Demo

This guide shows you exactly how to use the UnitTest Generator with this demo project.

## Demo 1: Generate Tests for a Single Class

1. **Open** the file `src/UTGenDemo.Core/Services/UserService.cs`
2. **Activate** the UnitTest chat mode (click the chat mode button and select "UnitTest")
3. **Type**: "Generate unit tests for this class"

**Expected Result**: The generator will create `tests/UTGenDemo.Tests/Services/UserServiceTests.cs` with tests for all methods including:
- Constructor tests (null argument validation)
- `GetUserByIdAsync` tests (valid ID, null/empty ID, user not found)
- `CreateUserAsync` tests (valid user, null user, invalid email, duplicate user)
- `GetActiveUsersAsync` tests
- And more...

## Demo 2: Generate Tests for Multiple Classes

1. **Activate** the UnitTest chat mode
2. **Type**: "Generate unit tests for the full project"

**Expected Result**: The generator will:
- Show you a plan of action
- Ask for confirmation
- Generate tests for ALL classes in the project:
  - `UserTests.cs` - Model tests
  - `ProductTests.cs` - Model with exception tests  
  - `UserServiceTests.cs` - Service with mocking tests
  - `ProductServiceTests.cs` - Complex business logic tests
  - `UsersControllerTests.cs` - Controller tests with HTTP context mocking

## Demo 3: Generate Tests for Changed Files

1. **Make a change** to any class (e.g., add a method to `UserService.cs`)
2. **Activate** the UnitTest chat mode
3. **Type**: "Generate unit tests" (without specifying files)

**Expected Result**: The generator will detect your changes and create tests only for the modified files.

## What You'll See in Generated Tests

### Example: UserService Constructor Test
```csharp
[Fact]
public void Constructor_NullUserRepository_ThrowsArgumentNullException()
{
    // Arrange & Act & Assert
    Should.Throw<ArgumentNullException>(() => 
        new UserService(null, _emailServiceMock.Object))
        .ParamName.ShouldBe("userRepository");
}
```

### Example: Async Method Test with Mocking
```csharp
[Fact]
public async Task GetUserByIdAsync_UserExists_ReturnsUser()
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
}
```

### Example: Exception Test
```csharp
[Fact]
public async Task CreateUserAsync_InvalidEmail_ThrowsArgumentException()
{
    // Arrange
    var user = new User { Email = "invalid-email" };
    
    // Act & Assert
    var exception = await Should.ThrowAsync<ArgumentException>(() => 
        _sut.CreateUserAsync(user));
    exception.ParamName.ShouldBe("user");
}
```

## Testing Features Demonstrated

✅ **Dependency Injection Mocking** - All services use Moq for dependencies  
✅ **Async/Await Testing** - Proper async test patterns  
✅ **Exception Testing** - Both expected and unexpected exceptions  
✅ **Edge Cases** - Null values, empty strings, boundary conditions  
✅ **Controller Testing** - HTTP context mocking, action results  
✅ **Business Logic** - Complex calculations and validations  
✅ **Repository Pattern** - Mocked data access layers  

## Tips for Best Results

1. **Keep classes focused** - Smaller classes generate more maintainable tests
2. **Use interfaces** - Dependencies should be interfaces for better mocking
3. **Include XML documentation** - Helps the generator understand intent
4. **Follow SOLID principles** - Makes testing and mocking easier

## Try It Now!

1. Pick any class from the demo project
2. Open the file in VS Code
3. Switch to UnitTest chat mode
4. Say "Generate unit tests for this class"
5. Watch the magic happen! ✨

The generator will create comprehensive, maintainable unit tests following all .NET testing best practices.
