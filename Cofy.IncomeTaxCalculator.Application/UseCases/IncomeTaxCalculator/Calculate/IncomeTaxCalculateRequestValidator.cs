using FluentValidation;

namespace Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate
{
    public class IncomeTaxCalculateRequestValidator : AbstractValidator<IncomeTaxCalculateRequest>
    {
        public IncomeTaxCalculateRequestValidator()
        {
            RuleFor(x => x.AnnualGrossSalary)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Annual salary must be greater than 0");
        }
    }
}
