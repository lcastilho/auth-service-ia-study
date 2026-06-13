using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RevokeRefreshToken;

public sealed record RevokeRefreshTokenCommand(string Token) : IRequest<Result>;
