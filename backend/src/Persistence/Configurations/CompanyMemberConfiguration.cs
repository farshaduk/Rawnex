using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.JobTitle).HasMaxLength(200);
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(m => new { m.CompanyId, m.UserId }).IsUnique();
        builder.HasIndex(m => m.UserId);

        builder.HasOne(m => m.User)
            .WithMany(u => u.CompanyMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
