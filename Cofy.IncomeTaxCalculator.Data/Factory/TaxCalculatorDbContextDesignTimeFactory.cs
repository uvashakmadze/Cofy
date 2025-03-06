using System.Diagnostics.CodeAnalysis;
using Cofy.IncomeTaxCalculator.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cofy.IncomeTaxCalculator.Data.Factory;

[ExcludeFromCodeCoverage]
public class TaxCalculatorDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TaxCalculatorDbContext>
{
    public TaxCalculatorDbContext CreateDbContext(string[] args)
    {
        var apiAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\Cofy.IncomeTaxCalculator.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiAssemblyPath)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var builder = new DbContextOptionsBuilder<TaxCalculatorDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Database"));

        return new TaxCalculatorDbContext(builder.Options);
    }
}
