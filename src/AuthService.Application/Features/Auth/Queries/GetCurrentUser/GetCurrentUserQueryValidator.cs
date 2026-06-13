using FluentValidation;

namespace AuthService.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty();
    }
}
