using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using FluentAssertions;

namespace AuthService.UnitTests.Domain;

public sealed class RefreshTokenTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ShouldCreateUsableToken_WhenDataIsValid()
    {
        var userId = Guid.NewGuid();
        var expiresAt = CreatedAt.AddDays(7);

        var refreshToken = RefreshToken.Create(userId, "refresh-token", expiresAt, CreatedAt);

        refreshToken.Id.Should().NotBeEmpty();
        refreshToken.UserId.Should().Be(userId);
        refreshToken.Token.Should().Be("refresh-token");
        refreshToken.CreatedAt.Should().Be(CreatedAt);
        refreshToken.ExpiresAt.Should().Be(expiresAt);
        refreshToken.RevokedAt.Should().BeNull();
        refreshToken.IsRevoked.Should().BeFalse();
        refreshToken.IsExpired(CreatedAt.AddDays(1)).Should().BeFalse();
        refreshToken.CanBeUsed(CreatedAt.AddDays(1)).Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenExpirationIsNotAfterCreation()
    {
        var act = () => RefreshToken.Create(Guid.NewGuid(), "refresh-token", CreatedAt, CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("Refresh token expiration must be after creation.");
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenCurrentTimeIsAfterExpiration()
    {
        var refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "refresh-token",
            CreatedAt.AddDays(7),
            CreatedAt);

        refreshToken.IsExpired(CreatedAt.AddDays(8)).Should().BeTrue();
        refreshToken.CanBeUsed(CreatedAt.AddDays(8)).Should().BeFalse();
    }

    [Fact]
    public void Revoke_ShouldMarkTokenAsRevoked()
    {
        var refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "refresh-token",
            CreatedAt.AddDays(7),
            CreatedAt);
        var revokedAt = CreatedAt.AddDays(1);

        refreshToken.Revoke(revokedAt);

        refreshToken.IsRevoked.Should().BeTrue();
        refreshToken.RevokedAt.Should().Be(revokedAt);
        refreshToken.CanBeUsed(revokedAt).Should().BeFalse();
    }

    [Fact]
    public void Revoke_ShouldThrowDomainException_WhenRevocationIsBeforeCreation()
    {
        var refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "refresh-token",
            CreatedAt.AddDays(7),
            CreatedAt);

        var act = () => refreshToken.Revoke(CreatedAt.AddMinutes(-1));

        act.Should().Throw<DomainException>()
            .WithMessage("Refresh token revocation cannot be before creation.");
    }
}
