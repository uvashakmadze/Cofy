namespace Cofy.IncomeTaxCalculator.Contracts.Models.Errors;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
}
