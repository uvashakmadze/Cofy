using System.Net.Mime;
using System.Security.Authentication;
using System.Text.Json;
using Cofy.IncomeTaxCalculator.API.Extensions;
using FluentValidation;

namespace Cofy.IncomeTaxCalculator.API.MiddleWares;

/// <summary>
/// Middleware for catching exceptions.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">Following delegate.</param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Forwards the http call to the destination.
    /// </summary>
    /// <param name="httpContext">Given HTTP context.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Handles the case when an exception is being thrown during a request execution.
    /// </summary>
    /// <param name="context">Given HTTP context.</param>
    /// <param name="exception">Given exception.</param>
    /// <returns><see cref="Task"/>.</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // We can add other exception types later (NotFound, Forbidden and so on)
        var error = exception switch
        {
            ValidationException ve => ve.ToBadRequest(),
            AuthenticationException u => u.ToAuthenticationAccess(),
            _ => exception.ToInternalServerError(),
        };

        context.Response.StatusCode = error.StatusCode;
        await context.Response.WriteAsJsonAsync(
            error,
            error.GetType(),
            (JsonSerializerOptions)null!,
            MediaTypeNames.Application.Json);
    }
}