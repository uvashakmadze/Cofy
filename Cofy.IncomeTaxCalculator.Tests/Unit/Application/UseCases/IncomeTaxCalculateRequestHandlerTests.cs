using Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Application.UseCases;

public class IncomeTaxCalculateRequestHandlerTests : BaseTestFixture
{
    private readonly IncomeTaxCalculateRequestHandler _incomeTaxCalculateRequestHandler = Substitute.For<IncomeTaxCalculateRequestHandler>();


    [TestCase(5000)]
    [TestCase(20000)]
    [TestCase(100000)]
    public async Task IncomeTaxCalculateRequestHandler_WhenDifferentSalaryRanges_CalculatesCorrectly(decimal amount)
    {
        // Arrange
        var incomeTaxCalculateRequest = new IncomeTaxCalculateRequest(amount);
        var grossMonthly = Math.Round(amount / 12, 2);
        var percent = CalculateIncomeTaxPercent(amount);
        var annualTax = Math.Round(amount / 100 * percent, 2);
        var monthlyTax = Math.Round(annualTax / 12, 2);
        var netAnnual = Math.Round(amount - annualTax, 2);
        var netMonthly = Math.Round(netAnnual / 12, 2);

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