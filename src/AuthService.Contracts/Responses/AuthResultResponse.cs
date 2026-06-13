namespace AuthService.Contracts.Responses;

public sealed record AuthResultResponse(
    Guid UserId,
    string Name,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);
