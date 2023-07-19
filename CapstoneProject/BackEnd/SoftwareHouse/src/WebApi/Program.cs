using System.Text;
using Application;
using Crawler.WorkerService;
using WebApi.Hubs;
using Domain.Settings;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Filters;


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add<GlobalExceptionFilter>();
        //opt.Filters.Add<ValidationFilter>();
    });

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));

    builder.Services.AddControllers(); 
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddSwaggerGen(setupAction =>
    {
        setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = $"Input your Bearer token in this format - Bearer token to access this API",
        });
        setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                }, new List<string>()
            },
        });
    });
    
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
            };
        });


    builder.Services.AddSignalR();

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.WebRootPath); 
    
    builder.Services.AddHttpClient(); 

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed((host) => true)
                .AllowAnyHeader());
    });

    builder.Services.AddHostedService<Worker>();
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseStaticFiles();
    
    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors("AllowAll");

    app.UseAuthorization();

    app.MapControllers();

    app.MapHub<CrawlerHub>("/Hubs/CrawlerHub");

    app.Run();
    
}
catch (Exception)
{
    Console.WriteLine("Application terminated unexpectedly");
}