using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Id)
            .ValueGeneratedNever();

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.Token)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(refreshToken => refreshToken.Token)
            .IsUnique();

        builder.Property(refreshToken => refreshToken.ExpiresAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.RevokedAt);

        builder.Ignore(refreshToken => refreshToken.IsRevoked);
    }
}
