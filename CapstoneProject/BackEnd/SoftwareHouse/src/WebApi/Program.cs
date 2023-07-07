using Application;
using Infrastrusture;
using Microsoft.EntityFrameworkCore;
using WebApi.Hubs;


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSignalR();

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructure(builder.Configuration); 
    
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
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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