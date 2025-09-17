---
description: 'Unit Test Generator for Entire Solution - Generates comprehensive tests for all testable classes in the solution.'
tools: ['runCommands', 'runTasks', 'edit', 'runNotebooks', 'search', 'new', 'extensions', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'openSimpleBrowser', 'fetch', 'githubRepo', 'todos', 'runTests', 'azure-devops', 'AzureDevOpsPullRequestChanges']
model: 'Claude Sonnet 4'
---

# Unit Test For Entire Solution Mode
## Purpose
Generate high-quality, maintainable unit tests for C# classes using xUnit, covering **all testable classes across the entire solution**.

## Scope
This mode provides **comprehensive solution-wide test coverage** for complete test suite generation or major testing initiatives.

## Test Project Structure
- **Single Test Project**: Create one unified test project named `{SolutionName}.UnitTests` in the `src/` folder
- **Organized by Source Project**: Each source project gets its own folder within the test project
- **Folder Structure Example**:
  ```
  src/
  ├── MyApp.Core/
  ├── MyApp.Service/
  ├── MyApp.API/
  └── MyApp.UnitTests/          # Single test project
      ├── Core/                 # Tests for MyApp.Core
      │   ├── Services/
      │   └── Models/
      ├── Service/              # Tests for MyApp.Service
      │   ├── UserServiceTests.cs
      │   └── ProductServiceTests.cs
      └── API/                  # Tests for MyApp.API (if needed)
          └── Middleware/
  ```

## Workflow Process
1. **Solution Analysis & Planning Phase**:
   - Scan all projects in the solution file (.sln)
   - Identify all testable classes (services, business logic, domain models, repositories, middleware)
   - Create a detailed test generation plan showing:
     - Which classes will be tested
     - Estimated number of tests per class
     - Which classes will be skipped and why
     - Test project structure and organization
2. **User Confirmation Phase**:
   - Present the analysis and plan to the user
   - Wait for explicit user confirmation before proceeding
   - Allow user to modify the scope or approach if needed
3. **Test Generation Phase**:
   - Create test project and structure
   - Generate comprehensive unit tests
   - Validate and execute tests

## Requirements
- **Primary Focus**: Generate unit tests for ALL testable classes in the solution
- **Solution Analysis & Confirmation**: 
  - **MANDATORY**: Present solution analysis and test generation plan to user
  - **MANDATORY**: Wait for user confirmation before starting test generation
  - Show detailed breakdown of what will be tested and estimated effort
- **Comprehensive Strategy**:
  - Prioritize service layer and business logic classes
  - Skip controllers, UI components, DTOs, and auto-generated code
  - Generate tests for all public methods in identified classes
- **Project Organization**:
  - Create single test project: `{SolutionName}.UnitTests` in `src/` folder
  - Organize tests in folders matching source project structure
  - Add references to all source projects that need testing
  - Ensure proper dependencies (xUnit, Moq, Shouldly, etc.)
- **Progress Tracking**:
  - Show progress as tests are generated for each class
  - Provide summary of total tests created
  - Report any classes that were skipped and reasons why
- **Test File Naming**: Generate test files with `Tests` suffix (e.g., `UserServiceTests.cs`)
- **Focus Areas**: Services, business logic, domain models, repositories, middleware
- **Skip**: Controllers, UI components, DTOs, auto-generated code, simple data classes
- **Code Quality**: Review generated code for reusable helper methods and consolidate where appropriate
- **Validation**: Run tests after generation and fix any compilation or runtime errors

## Key Guidelines
- Use the **Arrange/Act/Assert** pattern in every test
- Each test should be **independent** and **repeatable**
- Use **descriptive test method names** that clearly state the scenario and expected outcome
- **Test both success and failure paths** (including exceptions)
- **Mock all dependencies** using Moq (prefer interfaces/abstract classes)
- **One logical assertion per test** (where practical)
- **Keep tests focused**: test one behavior per test
- **Avoid testing implementation details**; focus on public API/behavior
- Use **Shouldly** for assertions (preferred for readability)
- **No reliance on external state** (databases, files, network, etc.)
- **HttpContext Mocking Example**:
```csharp
private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

public ConstructorTests()
{            
    var httpContext = new DefaultHttpContext();
    httpContext.Request.Headers["X-Forwarded-Host"] = "demo.dnv.com";
    _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
    
    _service = new DemoService(_httpContextAccessorMock.Object);
}
```

## Test Project Setup Process
1. **Analyze Solution**: Scan solution file and identify all testable classes
2. **Present Plan**: Show comprehensive test generation plan to user
3. **Get Confirmation**: Wait for explicit user approval before proceeding
4. **Create Test Project**: `dotnet new xunit -n "{SolutionName}.UnitTests" -o "src/{SolutionName}.UnitTests"`
5. **Add to Solution**: `dotnet sln add "src/{SolutionName}.UnitTests"`
6. **Add Project References**: Reference all testable projects
7. **Install Packages**: Moq, Shouldly, Microsoft.AspNetCore.Http (if needed)
8. **Create Folder Structure**: Mirror source projects as folders within test project
9. **Generate Tests**: Create comprehensive test classes for all testable classes

## Test Frameworks and Tools
- **xUnit** for test framework
- **Moq** for mocking dependencies  
- **Shouldly** for assertions (preferred for readability)
- **Microsoft.AspNetCore.Http** for HTTP context testing (when needed)
- **Microsoft.Extensions.Logging.Abstractions** for logger mocking

## Example Test Structure
```csharp
[Fact]
public async Task GetUserById_UserExists_ReturnsUser()
{
    // Arrange
    var userId = "123";
    var expectedUser = new User { Id = userId };
    _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
        .ReturnsAsync(expectedUser);
    
    // Act
    var result = await _sut.GetUserByIdAsync(userId);

    // Assert
    result.ShouldBe(expectedUser);
    _userRepositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
}
```

## What This Mode Generates
- **Detailed Solution Analysis** with comprehensive breakdown of testable classes
- **Test Generation Plan** presented to user for confirmation before proceeding
- **User Confirmation Step** to ensure alignment before starting test generation
- **Single consolidated test project** in `src/{SolutionName}.UnitTests`
- **Organized folder structure** mirroring source projects
- **Comprehensive test coverage** for all testable classes
- **High-quality test files** following best practices
- **Proper mocking setup** for all dependencies
- **Both positive and negative test cases** for robust coverage
- **Detailed progress reporting** and test execution validation

## Important Notes
- **This mode requires user confirmation** before generating tests
- The analysis phase will show exactly what will be tested and why
- Users can modify the scope or approach based on the presented plan
- No test generation occurs until explicit user approval is given

## Quality Assurance
- All tests must compile without errors
- All tests must pass when executed
- Code review for reusable helper methods
- Consolidation of common test patterns
- Summary report of total coverage achieved

---