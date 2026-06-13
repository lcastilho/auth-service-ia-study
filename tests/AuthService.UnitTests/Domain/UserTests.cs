using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using FluentAssertions;

namespace AuthService.UnitTests.Domain;

public sealed class UserTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ShouldCreateActiveUser_WhenDataIsValid()
    {
        var user = User.Create("Jane Doe", "JANE@Example.com", "hashed-password", CreatedAt);

        user.Id.Should().NotBeEmpty();
        user.Name.Should().Be("Jane Doe");
        user.Email.Should().Be("jane@example.com");
        user.PasswordHash.Should().Be("hashed-password");
        user.CreatedAt.Should().Be(CreatedAt);
        user.UpdatedAt.Should().BeNull();
        user.IsActive.Should().BeTrue();
        user.CanAuthenticate.Should().BeTrue();
        user.Roles.Should().BeEmpty();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenEmailIsInvalid()
    {
        var act = () => User.Create("Jane Doe", "invalid-email", "hashed-password", CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("User email format is invalid.");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenPasswordHashIsEmpty()
    {
        var act = () => User.Create("Jane Doe", "jane@example.com", " ", CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("Password hash is required.");
    }

    [Fact]
    public void Deactivate_ShouldPreventAuthentication()
    {
        var user = User.Create("Jane Doe", "jane@example.com", "hashed-password", CreatedAt);
        var updatedAt = CreatedAt.AddMinutes(5);

        user.Deactivate(updatedAt);

        user.IsActive.Should().BeFalse();
        user.CanAuthenticate.Should().BeFalse();
        user.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void AddRefreshToken_ShouldThrowDomainException_WhenTokenBelongsToAnotherUser()
    {
        var user = User.Create("Jane Doe", "jane@example.com", "hashed-password", CreatedAt);
        var refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "refresh-token",
            CreatedAt.AddDays(7),
            CreatedAt);

        var act = () => user.AddRefreshToken(refreshToken);

        act.Should().Throw<DomainException>()
            .WithMessage("Refresh token must belong to the user.");
    }
}
