using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class OrderApprovalConfiguration : IEntityTypeConfiguration<OrderApproval>
{
    public void Configure(EntityTypeBuilder<OrderApproval> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.StepName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Comments).HasMaxLength(1000);

        builder.HasIndex(a => a.PurchaseOrderId);

        builder.HasOne(a => a.ApproverUser)
            .WithMany()
            .HasForeignKey(a => a.ApproverUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
