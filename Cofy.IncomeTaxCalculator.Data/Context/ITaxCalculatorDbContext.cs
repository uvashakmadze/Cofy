using Cofy.IncomeTaxCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cofy.IncomeTaxCalculator.Data.Context;

public interface ITaxCalculatorDbContext
{
    DbSet<TaxBandEntity> TaxBands { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}