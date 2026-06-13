using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .ValueGeneratedNever();

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.Property(role => role.Description)
            .HasMaxLength(500);

        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "role_permissions",
                right => right
                    .HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("permission_id")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("role_id", "permission_id");
                    join.ToTable("role_permissions");
                });

        builder.Navigation(role => role.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
