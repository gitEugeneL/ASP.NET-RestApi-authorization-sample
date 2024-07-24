using System.Reflection;
using Application.Common.Interfaces;
using Application.Common.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        /*** Mediatr config ***/
        services.AddMediatR(cnf => 
            cnf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        /*** FluentValidation registration ***/
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
