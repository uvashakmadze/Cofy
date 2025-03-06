using Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate;
using Cofy.IncomeTaxCalculator.Data.Context;
using Cofy.IncomeTaxCalculator.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using MockQueryable.NSubstitute;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Application.UseCases;

public class IncomeTaxCalculateRequestHandlerTests : BaseTestFixture
{
    private readonly IMemoryCache _memoryCache = Substitute.For<IMemoryCache>();
    private readonly ITaxCalculatorDbContext _dbContext = Substitute.For<ITaxCalculatorDbContext>();
    private readonly IncomeTaxCalculateRequestHandler _incomeTaxCalculateRequestHandler;

    public IncomeTaxCalculateRequestHandlerTests()
    {
        _incomeTaxCalculateRequestHandler = Substitute.For<IncomeTaxCalculateRequestHandler>(_dbContext, _memoryCache);
    }

    [TestCase(5000, 0)]
    [TestCase(20000, 20)]
    [TestCase(100000, 40)]
    public async Task IncomeTaxCalculateRequestHandler_WhenDifferentSalaryRanges_CalculatesCorrectly(decimal amount, int percent)
    {
        // Arrange
        var incomeTaxCalculateRequest = new IncomeTaxCalculateRequest(amount);
        var grossMonthly = Math.Round(amount / 12, 2);
        var annualTax = Math.Round(amount / 100 * percent, 2);
        var monthlyTax = Math.Round(annualTax / 12, 2);
        var netAnnual = Math.Round(amount - annualTax, 2);
        var netMonthly = Math.Round(netAnnual / 12, 2);

        var dbSetMock = GenerateTaxBandsMock().AsQueryable().BuildMockDbSet();
        _dbContext.TaxBands.Returns(dbSetMock);

        // Act
        var response = await _incomeTaxCalculateRequestHandler.Handle(incomeTaxCalculateRequest, default);

        //Assert
        response.GrossAnnualSalary.Should().Be(amount);
        response.GrossMonthlySalary.Should().Be(grossMonthly);
        response.AnnualTaxPaid.Should().Be(annualTax);
        response.NetAnnualSalary.Should().Be(netAnnual);
        response.NetMonthlySalary.Should().Be(netMonthly);
        response.MonthlyTaxPaid.Should().Be(monthlyTax);
        response.IncomeTaxPercent.Should().Be(percent);
    }

    private static IEnumerable<TaxBandEntity> GenerateTaxBandsMock()
    {
        return new List<TaxBandEntity>
        {
            new() { Min = 0, Max = 5000, Percent = 0 },
            new() { Min = 5000, Max = 20000, Percent = 20 },
            new() { Min = 20000, Max = int.MaxValue, Percent = 40 },
        };
    }
}