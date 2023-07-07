using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastrusture.Persistence;
//infrastructure katmanında entityframeworkcore ve core.design kurduk, artı olarak pomelo kurduk mariadb kullanımı için.
public class ApplicationDbContext: DbContext,IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderEvent> OrderEvents { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }

}