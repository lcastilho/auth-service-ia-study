namespace AuthService.Application.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    Guid UserId,
    string Name,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);
