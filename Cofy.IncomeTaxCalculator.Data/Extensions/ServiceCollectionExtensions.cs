using Cofy.IncomeTaxCalculator.Data.Context;
using Cofy.IncomeTaxCalculator.Data.SeedWork.TaxBands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cofy.IncomeTaxCalculator.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaxCalculatorDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaxCalculatorDbContext>(opt =>
        {
            opt.SetupDatabaseProvider(configuration)
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        })
        .AddScoped<ITaxCalculatorDbContext, TaxCalculatorDbContext>();

        //Update Database if there are new migrations
        MigrateDatabase(services);

        //Run seeders
        RunSeeders(services);

        return services;
    }

    private static DbContextOptionsBuilder SetupDatabaseProvider(this DbContextOptionsBuilder builder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        builder.UseSqlServer(
            connectionString,
            opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return builder;
    }

    private static void MigrateDatabase(IServiceCollection services)
    {
        var context = services.BuildServiceProvider().GetService<TaxCalculatorDbContext>();
        //Update Database if there are new migrations
        try
        {
            context?.Database.Migrate();
        }
        catch
        {
            // ignored
        }
    }

    private static void RunSeeders(IServiceCollection services)
    {
        var context = services.BuildServiceProvider().GetService<TaxCalculatorDbContext>();
        if (context == null) return;
        var taxBandsSeeder = new TaxBandsSeeder(context);

        try
        {
            Task.Run(async () =>
            {
                await taxBandsSeeder.SeedAsync();
            });
        }
        catch
        {
            // ignored
        }
    }
}