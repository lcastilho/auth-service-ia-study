using AuthService.Application.Common;
using AuthService.Application.Interfaces.Repositories;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<CurrentUserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<CurrentUserResponse>.Failure("User was not found.");
        }

        var response = new CurrentUserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.IsActive,
            user.Roles.Select(role => role.Name).ToArray());

        return Result<CurrentUserResponse>.Success(response);
    }
}
