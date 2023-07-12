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
        builder.Property(x => x.ProductCrawlType).IsRequired()
            .HasConversion<int>();
        
        // Common Fields

        // CreatedOn
        builder.Property(x => x.CreatedOn).IsRequired();
        
        // CreatedByUserId
        builder.Property(x => x.CreatedByUserId).IsRequired(false)
            .HasMaxLength(100);

        // ModifiedOn
        builder.Property(x => x.ModifiedOn).IsRequired(false);
        
        // ModifiedByUserId
        builder.Property(x => x.ModifiedByUserId).IsRequired(false)
            .HasMaxLength(100);

        // DeletedOn
        builder.Property(x => x.DeletedOn).IsRequired(false);
        
        //DeletedByUserId
        builder.Property(x => x.DeletedByUserId).IsRequired(false)
            .HasMaxLength(100);

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
            .OnDelete(DeleteBehavior.Cascade);
        
        //User ile ilgili ilişki eklenebilir.
        
        builder.ToTable("Orders");
    }
}