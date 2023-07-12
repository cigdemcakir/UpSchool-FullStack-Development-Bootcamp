using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastrusture.Persistence.Configurations.Application; //burda yapılan her şey db için

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // ID
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        // Name
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);
        
        // Price
        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2");
        
        // SalePrice
        builder.Property(x => x.SalePrice)
            .IsRequired(false)
            .HasColumnType("decimal(18,2");
        
        // Picture
        builder.Property(x => x.Picture)
            .IsRequired()
            .HasMaxLength(500);
        
        //IsOnSale
        builder.Property(x => x.IsOnSale).IsRequired();
        
        // Common Fields

        // CreatedOn
        builder.Property(x => x.CreatedOn).IsRequired();
        
        //CreatedByUserId
        builder.Property(x => x.CreatedByUserId)
            .IsRequired(false)
            .HasMaxLength(100);

        // ModifiedOn
        builder.Property(x => x.ModifiedOn).IsRequired(false);
        
        //ModifiedByUserId
        builder.Property(x => x.ModifiedByUserId)
            .IsRequired(false)
            .HasMaxLength(100);

        // DeletedOn
        builder.Property(x => x.DeletedOn).IsRequired(false);
        
        //DeletedByUserId
        builder.Property(x => x.DeletedByUserId)
            .IsRequired(false)
            .HasMaxLength(100);

        // IsDeleted
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValueSql("0");
        builder.HasIndex(x => x.IsDeleted);

        // Relationships Product-Order
        builder.HasOne(x => x.Order)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Products");
    }
}