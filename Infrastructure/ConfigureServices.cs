using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("SQLServer")!;
        services.AddDbContext<DataContext>(option => 
            option.UseSqlServer(connection));
     
        return services;
    }
}
