using System.Security.Claims;
using System.Text;
using Api.Helpers;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Api;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        /*** Caret registration ***/
        services.AddCarter();
        
        /*** Auth config ***/
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(configuration.GetSection("Authentication:Key").Value!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1) // allowed time deviation, 5min - default
                };
            });

        /*** Auth policies ***/
        services.AddAuthorizationBuilder()
            .AddPolicy(AppConstants.BaseAuthPolicy, policy =>
                policy
                    .RequireClaim(ClaimTypes.Email)
                    .RequireClaim(ClaimTypes.NameIdentifier));
        
        /*** Swagger config ***/
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description =
                    """ Standard JWT Bearer Authorization with refresh token. Example: "Bearer" {your token} """,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
