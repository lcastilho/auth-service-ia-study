namespace AuthService.Contracts.Requests;

public sealed record RegisterUserRequest(
    string Name,
    string Email,
    string Password);
