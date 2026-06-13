namespace AuthService.Contracts.Responses;

public sealed record CurrentUserResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    IReadOnlyCollection<string> Roles);
