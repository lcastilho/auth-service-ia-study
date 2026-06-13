using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginResponse>>;
