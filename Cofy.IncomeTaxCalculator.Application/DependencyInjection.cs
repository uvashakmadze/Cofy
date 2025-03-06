using Cofy.IncomeTaxCalculator.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cofy.IncomeTaxCalculator.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            AssemblyScanner.FindValidatorsInAssembly(typeof(DependencyInjection).Assembly).ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            return services;
        }
    }
}
