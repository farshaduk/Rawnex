using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(d => d.CompanyId);

        builder.HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
