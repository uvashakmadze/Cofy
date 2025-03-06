namespace Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate
{
    public class IncomeTaxCalculateResponse
    {
        public decimal GrossAnnualSalary { get; set; }
        public decimal GrossMonthlySalary { get; set; }
        public decimal NetAnnualSalary { get; set; }
        public decimal NetMonthlySalary { get; set; }
        public decimal AnnualTaxPaid { get; set; }
        public decimal MonthlyTaxPaid { get; set; }
        public int IncomeTaxPercent { get; set; }
    }
}
