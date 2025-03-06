using System.Net;
using System.Text;

namespace Cofy.IncomeTaxCalculator.API.MiddleWares;

public class SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
{
    /// <summary>
    /// Handles HTTP requests that start with "/swagger" and validates if the request has a proper Basic Authentication header.
    /// If the username and password in the header are authorized, the middleware pipeline is invoked; otherwise, a 401 Unauthorized HTTP status code is returned.
    /// If the request does not start with "/swagger", the middleware pipeline is invoked directly.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/api/swagger"))
        {
            string authHeader = context.Request.Headers["Authorization"]!;
            if (authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                if (IsAuthorized(username, password))
                {
                    await next.Invoke(context);
                    return;
                }
            }

            context.Response.Headers["WWW-Authenticate"] = "Basic";

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            await next.Invoke(context);
        }
    }

    public bool IsAuthorized(string username, string password)
    {
        var configUserName = configuration["BasicAuthentication:UserName"];
        var configPassword = configuration["BasicAuthentication:Password"];

        return username.Equals(configUserName, StringComparison.InvariantCultureIgnoreCase) && password.Equals(configPassword);
    }
}