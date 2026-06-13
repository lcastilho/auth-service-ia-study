using AuthService.Application.Common;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Exceptions;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RevokeRefreshToken;

public sealed class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure("Refresh token was not found.");
        }

        try
        {
            refreshToken.Revoke(_dateTimeProvider.UtcNow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(exception.Message);
        }
    }
}
