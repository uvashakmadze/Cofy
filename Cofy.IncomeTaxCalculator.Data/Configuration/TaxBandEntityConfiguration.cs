using Cofy.IncomeTaxCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofy.IncomeTaxCalculator.Data.Configuration;

public class TaxBandEntityConfiguration : IEntityTypeConfiguration<TaxBandEntity>
{
    public void Configure(EntityTypeBuilder<TaxBandEntity> builder)
    {
        builder.ToTable("TaxBands");
        builder.HasKey(x => new { x.Min, x.Max, x.Percent });

        builder.Property(x => x.Min)
            .IsRequired();
        builder.Property(x => x.Max)
            .IsRequired();
        builder.Property(x => x.Percent)
            .IsRequired();

        builder.HasIndex(x => new { x.Min, x.Max, x.Percent })
            .IsUnique();
    }
}