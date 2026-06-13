using AuthService.Application.Common;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<Result<CurrentUserResponse>>;
