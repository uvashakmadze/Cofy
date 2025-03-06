using MediatR;

namespace Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate
{
    public class IncomeTaxCalculateRequestHandler: IRequestHandler<IncomeTaxCalculateRequest, IncomeTaxCalculateResponse>
    {
        public Task<IncomeTaxCalculateResponse> Handle(IncomeTaxCalculateRequest request, CancellationToken cancellationToken)
        {
            var percent = CalculateIncomeTaxPercent(request.AnnualGrossSalary);

            var grossAnnualSalary = request.AnnualGrossSalary;
            var grossMonthlySalary = Math.Round(grossAnnualSalary / 12, 2);
            var annualTax = Math.Round(grossAnnualSalary / 100 * percent, 2);
            var monthlyTax = Math.Round(annualTax / 12, 2);
            var netAnnualSalary = Math.Round(grossAnnualSalary - annualTax, 2);
            var netMonthlySalary = Math.Round(netAnnualSalary / 12, 2);

            var result = new IncomeTaxCalculateResponse
            {
                GrossAnnualSalary = grossAnnualSalary,
                GrossMonthlySalary = grossMonthlySalary,
                AnnualTaxPaid = annualTax,
                MonthlyTaxPaid = monthlyTax,
                NetAnnualSalary = netAnnualSalary,
                NetMonthlySalary = netMonthlySalary,
                IncomeTaxPercent = percent
            };

            return Task.FromResult(result);
        }

        private static int CalculateIncomeTaxPercent(decimal grossAnnualSalary)
        {
            return grossAnnualSalary switch
            {
                <= 5000 => 0,
                <= 20000 => 20,
                _ => 40
            };
        }
    }
}
