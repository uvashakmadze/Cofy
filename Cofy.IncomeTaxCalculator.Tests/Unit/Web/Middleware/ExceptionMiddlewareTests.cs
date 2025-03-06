using System.Net;
using System.Net.Mime;
using System.Security.Authentication;
using System.Text.Json;
using Cofy.IncomeTaxCalculator.API.MiddleWares;
using Cofy.IncomeTaxCalculator.Contracts.Models.Errors;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Web.Middleware;

public class ExceptionMiddlewareTests : BaseTestFixture, IDisposable
{
    private readonly ExceptionMiddleware _middleware;
    private readonly HttpContext _context;

    public ExceptionMiddlewareTests()
    {
        _middleware = new ExceptionMiddleware(Next);
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
        _context.Response.RegisterForDispose(_context.Response.Body);
    }

    public void Dispose()
    {
        _context.Response.OnCompleted(() => Task.CompletedTask);
    }

    [Test]
    public async Task ExceptionMiddleware_WhenInvokedAsync_ThenShouldForwardsHttpContextToNextDelegate()
    {
        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.Body.Should().NotBeNull();
        _context.Response.Body.Length.Should().Be(0);
    }

    [Test]
    public async Task ExceptionMiddleware_WhenExceptionIsThrown_ThenShouldRespondWithInternalServerError()
    {
        // Arrange
        Task Func() => throw new InvalidOperationException("Test error");
        var middleware = new ExceptionMiddleware(_ => Func());

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _context.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);

        var errorDetails = await DeserializeResponseAsync<ErrorDetails>(_context.Response);
        errorDetails.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        errorDetails.Message.Should().Be("Test error");
    }

    [Test]
    public async Task ExceptionMiddleware_WhenValidationExceptionIsThrown_ThenShouldRespondWithBadRequest()
    {
        // Arrange
        var errors = new List<ValidationFailure>()
        {
            new()
            {
                PropertyName = "foo",
                AttemptedValue = "bar",
                ErrorMessage = "Cannot be 'bar'",
            }
        };

        Task Func() => throw new ValidationException("Model not valid", errors);
        var middleware = new ExceptionMiddleware(_ => Func());

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _context.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);

        var errorDetails = await DeserializeResponseAsync<ModelNotValidErrorDetails>(_context.Response);
        errorDetails.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        errorDetails.Message.Should().Be("Model not valid");

        errorDetails.Errors.Should().HaveCount(1);
        errorDetails.Errors.First().Field.Should().Be("foo");
        errorDetails.Errors.First().Value.Should().Be("bar");
        errorDetails.Errors.First().Issue.Should().Be("Cannot be 'bar'");
    }

    [Test]
    public async Task ExceptionMiddleware_WhenAuthenticationExceptionIsThrown_ThenShouldRespondWith401Unauthorized()
    {
        // Arrange
        Func<Task> next = () => throw new AuthenticationException("Unauthorized access");
        var middleware = new ExceptionMiddleware(_ => next());

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        _context.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);

        var errorDetails = await DeserializeResponseAsync<ErrorDetails>(_context.Response);
        errorDetails.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        errorDetails.Message.Should().Be("Unauthorized access");
    }
    
    private static Task Next(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        return Task.CompletedTask;
    }

    private async Task<T> DeserializeResponseAsync<T>(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var streamReader = new StreamReader(_context.Response.Body);
        var responseBody = await streamReader.ReadToEndAsync();
        var body = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return body!;
    }
}