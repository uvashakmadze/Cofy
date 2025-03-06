namespace Cofy.IncomeTaxCalculator.Domain.Entities;

public class TaxBandEntity
{
    public int Min { get; set; }
    public int Max { get; set; }
    public byte Percent { get; set; }
}