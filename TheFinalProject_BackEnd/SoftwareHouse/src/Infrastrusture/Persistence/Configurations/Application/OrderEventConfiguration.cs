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

        // Status
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
        
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
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderEvents)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable("OrderEvents");
    }
}