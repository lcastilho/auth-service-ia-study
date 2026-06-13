using AuthService.Domain.Exceptions;

namespace AuthService.Domain.Entities;

public sealed class RefreshToken
{
    private RefreshToken()
    {
        Token = string.Empty;
    }

    private RefreshToken(Guid id, Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsExpired(DateTimeOffset utcNow)
    {
        return ExpiresAt <= utcNow;
    }

    public bool CanBeUsed(DateTimeOffset utcNow)
    {
        return !IsRevoked && !IsExpired(utcNow);
    }

    public static RefreshToken Create(Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("User id is required.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new DomainException("Refresh token value is required.");
        }

        if (expiresAt <= createdAt)
        {
            throw new DomainException("Refresh token expiration must be after creation.");
        }

        return new RefreshToken(Guid.NewGuid(), userId, token.Trim(), expiresAt, createdAt);
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        if (revokedAt < CreatedAt)
        {
            throw new DomainException("Refresh token revocation cannot be before creation.");
        }

        if (IsRevoked)
        {
            return;
        }

        RevokedAt = revokedAt;
    }
}
