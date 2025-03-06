using Cofy.IncomeTaxCalculator.API.OpenApi.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace Cofy.IncomeTaxCalculator.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerOpenApi(this IServiceCollection services, IConfiguration config)
    {
        // makes lowercased urls on swagger page
        services.Configure<RouteOptions>(opt =>
        {
            opt.LowercaseUrls = true;
            opt.LowercaseQueryStrings = true;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            var usedSchemaIds = new HashSet<string>();
            options.CustomSchemaIds(type =>
            {
                var name = type.Name;
                if (usedSchemaIds.Contains(name))
                {
                    // If name already exists, append last segment of namespace
                    name = type.Namespace?.Split('.').Last() + '.' + name;
                }
                usedSchemaIds.Add(name);
                return name;
            });

            options.AddSecurityDefinition(AuthenticationSchemes.Basic, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = AuthenticationSchemes.Basic,
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Basic scheme.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = AuthenticationSchemes.Basic,
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        return services;
    }

    public static IServiceCollection AddBasicAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationSchemes.Basic, null);
        return services;
    }
}
