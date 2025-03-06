using Cofy.IncomeTaxCalculator.Data.Context;
using Cofy.IncomeTaxCalculator.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate
{
    public class IncomeTaxCalculateRequestHandler(ITaxCalculatorDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<IncomeTaxCalculateRequest, IncomeTaxCalculateResponse>
    {
        const string CacheKey = "IncomeTaxBandsKey";
        public async Task<IncomeTaxCalculateResponse> Handle(IncomeTaxCalculateRequest request, CancellationToken cancellationToken)
        {
            var percent = await CalculateIncomeTaxPercentAsync(request.AnnualGrossSalary);

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

            return result;
        }

        private async Task<int> CalculateIncomeTaxPercentAsync(decimal grossAnnualSalary)
        {
            if (!memoryCache.TryGetValue<List<TaxBandEntity>>(CacheKey, out var bands))
            {
                bands = await dbContext.TaxBands.ToListAsync();
                SaveTaxBandsIntoCache(bands);
            }

            if (bands == null) return 0;

            return (
                from
                    band in bands
                where
                    grossAnnualSalary > band.Min && grossAnnualSalary <= band.Max
                select
                    band.Percent)
                .FirstOrDefault();
        }

        private void SaveTaxBandsIntoCache(List<TaxBandEntity> bands)
        {
            memoryCache.Remove(CacheKey);
            var expirationDate = DateTime.Now.AddHours(1);

            MemoryCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpiration = expirationDate
            };

            memoryCache.Set(CacheKey, bands, cacheOptions);
        }
    }
}
