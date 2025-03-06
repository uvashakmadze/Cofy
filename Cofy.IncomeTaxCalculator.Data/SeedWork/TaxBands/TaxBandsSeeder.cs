using System.Diagnostics.CodeAnalysis;
using Cofy.IncomeTaxCalculator.Data.Context;
using Cofy.IncomeTaxCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cofy.IncomeTaxCalculator.Data.SeedWork.TaxBands;

[ExcludeFromCodeCoverage]
public class TaxBandsSeeder(ITaxCalculatorDbContext context)
{
    public async Task SeedAsync()
    {
        var bands = new List<TaxBandEntity>
        {
            new() { Min = 0, Max = 5000, Percent = 0 },
            new() { Min = 5000, Max = 20000, Percent = 20 },
            new() { Min = 20000, Max = int.MaxValue, Percent = 40 },
        };

        var validBands = await context.TaxBands.Select(x => new { x.Percent, x.Min, x.Max }).ToListAsync();
        var bandsToSave = bands.Where(b => !validBands.Contains(new { b.Percent, b.Min, b.Max }));

        await context.TaxBands.AddRangeAsync(bandsToSave);
        await context.SaveChangesAsync();
    }
}