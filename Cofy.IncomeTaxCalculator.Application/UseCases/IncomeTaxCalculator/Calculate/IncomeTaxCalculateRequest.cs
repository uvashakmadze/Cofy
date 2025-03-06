using MediatR;

namespace Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate
{
    public class IncomeTaxCalculateRequest(decimal annualGrossSalary) : IRequest<IncomeTaxCalculateResponse>
    {
        public decimal AnnualGrossSalary { get; } = annualGrossSalary;
    }
}
