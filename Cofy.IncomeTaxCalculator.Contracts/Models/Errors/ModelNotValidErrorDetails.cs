namespace Cofy.IncomeTaxCalculator.Contracts.Models.Errors;

public class ModelNotValidErrorDetails : ErrorDetails
{
    public IEnumerable<ValidationError> Errors { get; set; } = null!;
}
