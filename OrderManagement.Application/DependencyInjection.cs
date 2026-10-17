

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Interface.RepositoryContracts;
using OrderManagement.Application.Interface.ServiceContracts;
using OrderManagement.Application.Services;

namespace OrderManagement.Application;
/// <summary>
/// method to inject service collection from program.cs
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
