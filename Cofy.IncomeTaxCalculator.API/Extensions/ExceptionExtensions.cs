using System.Net;
using System.Security.Authentication;
using Cofy.IncomeTaxCalculator.Contracts.Models.Errors;
using FluentValidation;

namespace Cofy.IncomeTaxCalculator.API.Extensions;

public static class ExceptionExtensions
{
    public static ErrorDetails ToBadRequest(this Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ModelNotValidErrorDetails
            {
                Message = "Model not valid",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Errors = validationException.Errors.Select(error => new ValidationError
                {
                    Field = error.PropertyName,
                    Issue = error.ErrorMessage,
                    Value = error.AttemptedValue?.ToString() ?? string.Empty,
                }),
            },
            _ => new ErrorDetails { Message = exception.Message, StatusCode = (int)HttpStatusCode.BadRequest, }
        };
    }

    public static ErrorDetails ToInternalServerError(this Exception exception)
    {
        return new ErrorDetails
        {
            Message = exception.Message,
            StatusCode = (int)HttpStatusCode.InternalServerError,
        };
    }

    public static ErrorDetails ToAuthenticationAccess(this AuthenticationException exception)
    {
        return new ErrorDetails
        {
            Message = exception.Message,
            StatusCode = (int)HttpStatusCode.Unauthorized,
        };
    }
}