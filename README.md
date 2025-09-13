# UTGen Demo Project

This is a demonstration project to showcase the **UnitTest Generator** chat mode capabilities. The project contains various C# classes with different testing scenarios to demonstrate comprehensive unit test generation.

## Project Structure

```
UTGen Demo/
├── .github/
│   └── chatmodes/
│       └── UnitTest.chatmode.md          # The unit test generator configuration
├── src/
│   ├── UTGenDemo.Api/                    # Web API project
│   │   └── Controllers/
│   │       └── UsersController.cs        # API controller with error handling
│   └── UTGenDemo.Core/                   # Core business logic library
│       ├── Models/
│       │   ├── User.cs                   # Simple model with validation
│       │   └── Product.cs                # Model with business logic & exceptions
│       ├── Interfaces/
│       │   ├── IUserRepository.cs        # Repository interface
│       │   ├── IProductRepository.cs     # Repository interface
│       │   └── IEmailService.cs          # Email service interface
│       └── Services/
│           ├── UserService.cs            # Service with dependency injection
│           └── ProductService.cs         # Service with complex business logic
└── tests/
    └── UTGenDemo.Tests/                  # Test project (ready for generated tests)
```

## Demo Classes and Testing Scenarios

### 1. **User.cs** - Simple Model Testing
- Properties validation
- Computed properties (`FullName`)
- Simple business logic (`IsEmailValid`)
- **Perfect for:** Basic unit testing patterns

### 2. **Product.cs** - Business Logic with Exceptions
- Exception handling (`CalculateDiscountPrice`)
- Edge cases (negative discounts, out of range)
- Boolean logic (`IsInStock`)
- **Perfect for:** Testing exception scenarios and edge cases

### 3. **UserService.cs** - Service with Dependencies
- Constructor dependency injection
- Async operations
- Multiple dependencies (repository + email service)
- Complex validation logic
- Exception handling and business rules
- **Perfect for:** Dependency mocking and async testing

### 4. **ProductService.cs** - Complex Business Logic
- LINQ operations
- Null handling
- Collection processing
- Error propagation
- **Perfect for:** Complex business logic testing

### 5. **UsersController.cs** - API Controller Testing
- HTTP context testing
- Action result testing
- Error response handling
- Logging verification
- **Perfect for:** Controller testing with mocking HttpContext

## How to Use This Demo

### Option 1: Generate Tests for a Specific Class
1. Open any class file (e.g., `UserService.cs`)
2. Activate the UnitTest chat mode
3. Say: "Generate unit tests for this class"
4. The generator will create comprehensive tests for all methods

### Option 2: Generate Tests for the Entire Project
1. Activate the UnitTest chat mode
2. Say: "Generate unit tests for the full project"
3. The generator will create a plan and generate tests for all classes

### Option 3: Generate Tests for Changed Files
1. Make some changes to the code
2. Activate the UnitTest chat mode  
3. Say: "Generate unit tests" (without specifying files)
4. The generator will create tests for changed files since the last pull

## Pre-configured Testing Dependencies

The test project (`UTGenDemo.Tests`) is already configured with:
- ✅ **xUnit** - Test framework
- ✅ **Moq** - Mocking framework
- ✅ **Shouldly** - Fluent assertions
- ✅ Project references to Core and API projects

## Expected Test Patterns

The generated tests will follow these patterns:
- ✅ **Arrange/Act/Assert** structure
- ✅ **Descriptive test names** (e.g., `GetUserById_UserExists_ReturnsUser`)
- ✅ **Dependency mocking** using Moq
- ✅ **Async/await testing** for async methods
- ✅ **Exception testing** for error scenarios
- ✅ **Edge case coverage** (null values, empty strings, boundary conditions)
- ✅ **Independent tests** (no shared state)

## Build and Test

```bash
# Build the solution
dotnet build

# Run all tests (after generation)
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Key Testing Scenarios Demonstrated

1. **Simple Property Testing** (`User` model)
2. **Business Logic with Exceptions** (`Product.CalculateDiscountPrice`)
3. **Dependency Injection Mocking** (`UserService` constructor)
4. **Async Method Testing** (All service methods)
5. **Repository Pattern Mocking** (All service dependencies)
6. **Controller Testing** (`UsersController` with HTTP context)
7. **Logging Verification** (Controller logging behavior)
8. **Collection Processing** (`ProductService.CalculateTotalValue`)
9. **Error Propagation** (Service exception handling)
10. **Input Validation** (Multiple validation scenarios)

This demo provides a comprehensive set of scenarios to showcase the unit test generator's capabilities across different types of C# code patterns.
