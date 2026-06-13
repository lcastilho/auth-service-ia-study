using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password) : IRequest<Result<RegisterUserResponse>>;
