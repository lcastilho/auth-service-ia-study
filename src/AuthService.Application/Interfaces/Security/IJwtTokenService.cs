using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Security;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(User user);
    RefreshTokenResult CreateRefreshToken();
}

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAt);

public sealed record RefreshTokenResult(string Token, DateTimeOffset ExpiresAt);
