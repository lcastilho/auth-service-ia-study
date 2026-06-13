using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt);

        builder.Ignore(user => user.CanAuthenticate);

        builder.HasMany(user => user.Roles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                right => right
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("user_id", "role_id");
                    join.ToTable("user_roles");
                });

        builder.Navigation(user => user.Roles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(user => user.RefreshTokens)
            .WithOne()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
