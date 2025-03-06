using System.Text.Encodings.Web;
using Cofy.IncomeTaxCalculator.API.OpenApi.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Web.Handlers;

public class BasicAuthenticationHandlerTests : BaseTestFixture
{
    private readonly BasicAuthenticationHandler _handler;

    public BasicAuthenticationHandlerTests()
    {
        // Setup
        var options = Substitute.For<IOptionsMonitor<AuthenticationSchemeOptions>>();

        options.Get(Arg.Is<string>(x => x == AuthenticationSchemes.Basic))
            .Returns(new AuthenticationSchemeOptions());

        var logger = Substitute.For<ILogger<BasicAuthenticationHandler>>();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(string.Empty).Returns(logger);

        var encoder = Substitute.For<UrlEncoder>();
        
        var inMemorySettings = new Dictionary<string, string> {
            {"BasicAuthentication:UserName", "TestUserName"},
            {"BasicAuthentication:Password", "TestPassword"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _handler = new BasicAuthenticationHandler(options, loggerFactory, encoder, configuration);
    }

    [Test]
    public async Task HandleAuthenticateAsync_WhenCredentialsAreEmpty_ReturnsAuthenticateResultFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues(string.Empty);
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme(AuthenticationSchemes.Basic, AuthenticationSchemes.Basic, typeof(BasicAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Be("Basic authentication failed. Invalid Authorization Header.");
    }

    [Test]
    public async Task HandleAuthenticateAsync_WhenCredentialsAreIncorrect_ReturnsAuthenticateResultFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Basic XYTzdFBvDXJOY7LlOlRlc3RQYXNzd29yPQ+=");
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme(AuthenticationSchemes.Basic, null, typeof(BasicAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Be("Basic authentication failed. Invalid Username Or Password.");
    }

    [Test]
    public async Task HandleAuthenticateAsync_WhenPrincipalIsNull_ReturnsAuthenticateResultSuccessInTicket()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Basic VGVzdFVzZXJOYW1lOlRlc3RQYXNzd29yZA==");
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme(AuthenticationSchemes.Basic, null, typeof(BasicAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Ticket!.AuthenticationScheme.Should().Be(AuthenticationSchemes.Basic);
    }
}