using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Services;
using Infrastrusture.Persistence;
using Infrastrusture.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastrusture;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MariaDB");

        services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        services.AddDbContext<IdentityContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        
        //Scoped Services
        services.AddScoped<IApplicationDbContext>(provider=>provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<IAuthenticationService, AuthenticationManager>();
        
        //Singleton Services
        services.AddSingleton<IJwtService, JwtManager>();

        services.AddSingleton<IEmailService, EmailManager>();

        return services;
    }
}