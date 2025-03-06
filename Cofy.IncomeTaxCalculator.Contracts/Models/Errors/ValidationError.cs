namespace Cofy.IncomeTaxCalculator.Contracts.Models.Errors;

public class ValidationError
{
    public string Field { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Issue { get; set; } = null!;
}