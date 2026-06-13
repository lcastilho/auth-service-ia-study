namespace AuthService.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record CurrentUserResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    IReadOnlyCollection<string> Roles);
