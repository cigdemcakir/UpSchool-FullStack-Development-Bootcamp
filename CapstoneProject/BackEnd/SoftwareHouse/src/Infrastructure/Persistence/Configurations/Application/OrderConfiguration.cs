using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastrusture.Persistence.Configurations.Application;

public class OrderConfiguration:IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // ID
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // RequestedAmount
        builder.Property(x => x.RequestedAmount).IsRequired();
        
        // TotalFoundAmount
        builder.Property(x => x.TotalFoundAmount).IsRequired();

        // ProductCrawlType
        builder.Property(x => x.ProductCrawlType).IsRequired()
            .HasConversion<int>();
        
        // Common Fields

        // CreatedOn
        builder.Property(x => x.CreatedOn).IsRequired();
        
        // CreatedByUserId
        builder.Property(x => x.CreatedByUserId).IsRequired(false)
            .HasMaxLength(100);

        // Relationships 
        builder.HasMany(x => x.OrderEvents)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //User ile ilgili ilişki eklenebilir.
        
        builder.ToTable("Orders");
    }
}