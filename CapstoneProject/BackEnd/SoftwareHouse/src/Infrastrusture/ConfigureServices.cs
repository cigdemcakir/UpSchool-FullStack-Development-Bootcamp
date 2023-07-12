using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Domain.Identity;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Services;
using Infrastrusture.Persistence;
using Infrastrusture.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace Infrastrusture;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string environmentWebRootPath)
    {
        var connectionString = configuration.GetConnectionString("MariaDB");

        services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        services.AddDbContext<IdentityContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        services.AddIdentity<User, Role>(options =>
            {

                // User Password Options
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                // User Username and Email Options
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+$";
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        //Scoped Services
        services.AddScoped<IApplicationDbContext>(provider=>provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<IAuthenticationService, AuthenticationManager>();
        
        //Singleton Services
        services.AddSingleton<IJwtService, JwtManager>();

        services.AddSingleton<IEmailService, EmailManager>();
        
        services.AddScoped<IAuthenticationService, AuthenticationManager>();

        return services;
    }
}