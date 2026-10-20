using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.ConnectionFactory;
using OrderManagement.Infrastructure.Repository;
using OrderManagement.Application.Interface.RepositoryContracts;

namespace OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("default");

        services.AddSingleton<SqlConnectionFactory>(
      new SqlConnectionFactory(connectionString!)
  );

        // Register connection factory/repository here

        services.AddScoped<IProductRepo, ProductRepo>();

        services.AddScoped<IUserStore, UserStore>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}