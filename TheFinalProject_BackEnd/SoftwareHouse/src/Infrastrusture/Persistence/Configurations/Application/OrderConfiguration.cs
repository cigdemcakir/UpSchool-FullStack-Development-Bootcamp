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

        // RequestedAmount
        builder.Property(x => x.RequestedAmount).IsRequired();
        
        // TotalFoundAmound
        builder.Property(x => x.TotalFoundAmount).IsRequired();

        // ProductCrawlType
        builder.Property(x => x.ProductCrawlType).IsRequired();
        builder.Property(x => x.ProductCrawlType).HasConversion<int>();
        
        // Common Fields

        // CreatedOn
        builder.Property(x => x.CreatedOn).IsRequired();

        // ModifiedOn
        builder.Property(x => x.ModifiedOn).IsRequired(false);

        // DeletedOn
        builder.Property(x => x.DeletedOn).IsRequired(false);

        // IsDeleted
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValueSql("0");
        builder.HasIndex(x => x.IsDeleted);

        // Relationships 
        builder.HasMany(x => x.OrderEvents)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Orders");
    }
}