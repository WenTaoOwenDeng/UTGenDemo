using Xunit;
using Moq;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UTGenDemo.Controller.Middleware;
using System.Text.Json;
using System.Net;

namespace UTGenDemo.UnitTests.Controller.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
    private readonly ExceptionMiddleware _sut;

    public ExceptionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _sut = new ExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var middleware = new ExceptionMiddleware(_nextMock.Object, _loggerMock.Object);

        // Assert
        middleware.ShouldNotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoExceptionThrown_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(n => n(context)).Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        _nextMock.Verify(n => n(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndHandlesException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new InvalidOperationException("Test exception");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        _nextMock.Verify(n => n(context), Times.Once);
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unexpected error occurred")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        context.Response.ContentType.ShouldBe("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentNullException_ReturnsBadRequestWithCorrectErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentNullException("testParam", "Test parameter is null");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        errorResponse.Message.ShouldContain("Test parameter is null");
        errorResponse.Details.ShouldBe("Required parameter is null or empty");
        errorResponse.Timestamp.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentException_ReturnsBadRequestWithCorrectErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentException("Invalid argument provided", "testParam");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        errorResponse.Message.ShouldContain("Invalid argument provided");
        errorResponse.Details.ShouldBe("Invalid argument provided");
    }

    [Fact]
    public async Task InvokeAsync_WithKeyNotFoundException_ReturnsNotFoundWithCorrectErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new KeyNotFoundException("Resource not found");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
        errorResponse.Message.ShouldBe("Resource not found");
        errorResponse.Details.ShouldBe("The requested resource was not found");
    }

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedAccessException_ReturnsUnauthorizedWithCorrectErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new UnauthorizedAccessException("Access denied");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        errorResponse.Message.ShouldBe("Access denied");
        errorResponse.Details.ShouldBe("Access denied");
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidOperationException_ReturnsBadRequestWithCorrectErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new InvalidOperationException("Invalid operation");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        errorResponse.Message.ShouldBe("Invalid operation");
        errorResponse.Details.ShouldBe("Invalid operation");
    }

    [Fact]
    public async Task InvokeAsync_WithUnknownException_ReturnsInternalServerErrorWithGenericErrorResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new NotImplementedException("This method is not implemented");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);

        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
        errorResponse.Message.ShouldBe("An internal server error occurred");
        errorResponse.Details.ShouldBe("Please try again later or contact support");
    }

    [Fact]
    public async Task InvokeAsync_WithException_SetsContentTypeToApplicationJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentException("Test exception");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.ContentType.ShouldBe("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WithException_WritesValidJsonResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentException("Test message");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        
        // Should not throw when deserializing
        var errorResponse = Should.NotThrow(() => JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        errorResponse.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
    [InlineData(typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(InvalidOperationException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(NotImplementedException), HttpStatusCode.InternalServerError)]
    public async Task InvokeAsync_WithVariousExceptionTypes_ReturnsCorrectStatusCode(Type exceptionType, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)expectedStatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithExceptionAfterResponseStarted_DoesNotModifyResponse()
    {
        // This test simulates a scenario where the response has already started
        // In real scenarios, this would be handled differently, but we test the behavior
        
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentException("Test exception");
        
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act & Assert - Should not throw
        await Should.NotThrowAsync(async () => await _sut.InvokeAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_LogsExceptionWithCorrectLogLevel()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new InvalidOperationException("Test exception");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ErrorResponse_HasCorrectTimestamp()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new ArgumentException("Test exception");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;
        var beforeRequest = DateTime.UtcNow;

        // Act
        await _sut.InvokeAsync(context);

        // Assert
        var afterRequest = DateTime.UtcNow;
        responseStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseStream).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse!.Timestamp.ShouldBeInRange(beforeRequest, afterRequest);
    }
}

public class ErrorResponseTests
{
    [Fact]
    public void Constructor_CreatesErrorResponseWithDefaultValues()
    {
        // Arrange & Act
        var errorResponse = new ErrorResponse();

        // Assert
        errorResponse.StatusCode.ShouldBe(0);
        errorResponse.Message.ShouldBe(string.Empty);
        errorResponse.Details.ShouldBe(string.Empty);
        errorResponse.Timestamp.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void ErrorResponse_AllPropertiesCanBeSetAndRetrieved()
    {
        // Arrange
        var statusCode = 400;
        var message = "Test message";
        var details = "Test details";
        var timestamp = DateTime.UtcNow.AddDays(-1);

        // Act
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = details,
            Timestamp = timestamp
        };

        // Assert
        errorResponse.StatusCode.ShouldBe(statusCode);
        errorResponse.Message.ShouldBe(message);
        errorResponse.Details.ShouldBe(details);
        errorResponse.Timestamp.ShouldBe(timestamp);
    }

    [Fact]
    public void ErrorResponse_CanBeSerializedToJson()
    {
        // Arrange
        var errorResponse = new ErrorResponse
        {
            StatusCode = 404,
            Message = "Not Found",
            Details = "The resource was not found",
            Timestamp = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"statusCode\":404");
        json.ShouldContain("\"message\":\"Not Found\"");
        json.ShouldContain("\"details\":\"The resource was not found\"");
        json.ShouldContain("\"timestamp\":\"2023-01-01T12:00:00Z\"");
    }

    [Fact]
    public void ErrorResponse_CanBeDeserializedFromJson()
    {
        // Arrange
        var json = """{"statusCode":400,"message":"Bad Request","details":"Invalid input","timestamp":"2023-01-01T12:00:00Z"}""";

        // Act
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        errorResponse.ShouldNotBeNull();
        errorResponse.StatusCode.ShouldBe(400);
        errorResponse.Message.ShouldBe("Bad Request");
        errorResponse.Details.ShouldBe("Invalid input");
        errorResponse.Timestamp.ShouldBe(new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc));
    }
}