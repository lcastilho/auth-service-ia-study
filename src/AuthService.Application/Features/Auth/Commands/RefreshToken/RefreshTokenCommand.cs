using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token) : IRequest<Result<RefreshTokenResponse>>;
