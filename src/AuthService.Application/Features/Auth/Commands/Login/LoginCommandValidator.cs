using FluentValidation;

namespace AuthService.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
