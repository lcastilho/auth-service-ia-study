using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Id)
            .ValueGeneratedNever();

        builder.Property(permission => permission.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(permission => permission.Name)
            .IsUnique();

        builder.Property(permission => permission.Description)
            .HasMaxLength(500);
    }
}
