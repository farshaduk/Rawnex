using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ContractClauseConfiguration : IEntityTypeConfiguration<ContractClause>
{
    public void Configure(EntityTypeBuilder<ContractClause> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(300);
        builder.Property(c => c.Content).IsRequired().HasMaxLength(8000);

        builder.HasIndex(c => c.ContractId);
    }
}
