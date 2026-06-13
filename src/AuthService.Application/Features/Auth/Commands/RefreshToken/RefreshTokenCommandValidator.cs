using FluentValidation;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty();
    }
}
