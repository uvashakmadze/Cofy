using System.Reflection;
using Cofy.IncomeTaxCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cofy.IncomeTaxCalculator.Data.Context;

public class TaxCalculatorDbContext(DbContextOptions<TaxCalculatorDbContext> options) : DbContext(options),
    ITaxCalculatorDbContext
{
    public DbSet<TaxBandEntity> TaxBands { get; set; }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}