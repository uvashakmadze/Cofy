using Cofy.IncomeTaxCalculator.Data.Context;
using Cofy.IncomeTaxCalculator.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cofy.IncomeTaxCalculator.Tests.Unit.Data.Extensions;

public class TaxCalculatorDbContextConfigurationTests : BaseTestFixture
{
    [Test]
    public void TaxCalculatorDbContext_WhenDevelopmentEnvironment_DbContextShouldUseSqlServerProvider()
    {
        // Arrange
        var config = new Dictionary<string, string>
        {
            ["ConnectionStrings:Database"] = "sql_server_db_connection_string",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config!)
            .Build();

        var serviceCollection = new ServiceCollection()
            .AddTaxCalculatorDbContext(configuration);

        // Act
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var context = serviceProvider.GetService<TaxCalculatorDbContext>();

        // Assert
        context.Should().NotBeNull();
        var databaseConnectionString = context.Database.GetConnectionString();
        databaseConnectionString.Should().Be("sql_server_db_connection_string");
        context.Database.IsRelational().Should().BeTrue();
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.SqlServer");
    }
}
