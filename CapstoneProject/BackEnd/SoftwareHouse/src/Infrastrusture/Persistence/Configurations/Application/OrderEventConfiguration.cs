using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastrusture.Persistence.Configurations.Application;

public class OrderEventConfiguration:IEntityTypeConfiguration<OrderEvent> // kontrol et!
{
    public void Configure(EntityTypeBuilder<OrderEvent> builder)
    {
        // ID
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Status
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
        
        // Common Fields

        // CreatedOn
        builder.Property(x => x.CreatedOn).IsRequired();
        
        // CreatedByUserId
        builder.Property(x => x.CreatedByUserId)
            .IsRequired(false)
            .HasMaxLength(100);

        // ModifiedOn
        builder.Property(x => x.ModifiedOn).IsRequired(false);

        // ModifiedByUserId
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

        // Relationships
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderEvents)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable("OrderEvents");
    }
}