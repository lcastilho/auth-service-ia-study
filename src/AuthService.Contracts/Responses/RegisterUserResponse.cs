namespace AuthService.Contracts.Responses;

public sealed record RegisterUserResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAt);
