using System.Net;
using System.Text;
using Cofy.IncomeTaxCalculator.API.MiddleWares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Web.Middleware;

[TestFixture]
public class SwaggerBasicAuthMiddlewareTests : BaseTestFixture, IDisposable
{
    private readonly SwaggerBasicAuthMiddleware _middleware;
    private readonly HttpContext _context;

    public SwaggerBasicAuthMiddlewareTests()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new List<KeyValuePair<string, string>>
            {
                new ("BasicAuthentication:UserName", "testUser"),
                new ("BasicAuthentication:Password", "testPass"),
            }!)
            .Build();

        _middleware = new SwaggerBasicAuthMiddleware(next, config);

        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    public void Dispose()
    {
        _context.Response.Body.Close();
    }

    [TestCase("testUser", "testPass", "/swagger", (int)HttpStatusCode.OK)]
    [TestCase("testUser", "testPass", "/api/swagger", (int)HttpStatusCode.OK)]
    [TestCase("wrongUser", "wrongPass", "/swagger", (int)HttpStatusCode.Unauthorized)]
    [TestCase("wrongUser", "wrongPass", "/api/swagger", (int)HttpStatusCode.Unauthorized)]
    public async Task TestMiddleware_WhenDifferentTestCases_ReturnsAppropriateStatusCode(string username, string password, string path, int expectedResult)
    {
        // Arrange
        _context.Request.Path = path;

        // Act
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var authInfo = $"{username}:{password}";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            _context.Request.Headers["Authorization"] = $"Basic {authInfo}";
        }
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(expectedResult);
    }
}
