using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(role => role.Value)
            .IsUnique();
        
        builder.Property(role => role.Value)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();
    }
}
