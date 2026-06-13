using FluentValidation;

namespace AuthService.Application.Features.Auth.Commands.RevokeRefreshToken;

public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty();
    }
}
