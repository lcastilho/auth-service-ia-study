using AuthService.Application.Common;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Security;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Exceptions;
using MediatR;
using DomainRefreshToken = AuthService.Domain.Entities.RefreshToken;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var currentRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Token, cancellationToken);
        var utcNow = _dateTimeProvider.UtcNow;

        if (currentRefreshToken is null || !currentRefreshToken.CanBeUsed(utcNow))
        {
            return Result<RefreshTokenResponse>.Failure("Refresh token is invalid.");
        }

        var user = await _userRepository.GetByIdAsync(currentRefreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result<RefreshTokenResponse>.Failure("Refresh token user was not found.");
        }

        if (!user.CanAuthenticate)
        {
            return Result<RefreshTokenResponse>.Failure("User is inactive.");
        }

        try
        {
            currentRefreshToken.Revoke(utcNow);

            var accessToken = _jwtTokenService.CreateAccessToken(user);
            var newRefreshTokenResult = _jwtTokenService.CreateRefreshToken();
            var newRefreshToken = DomainRefreshToken.Create(
                user.Id,
                newRefreshTokenResult.Token,
                newRefreshTokenResult.ExpiresAt,
                utcNow);

            user.AddRefreshToken(newRefreshToken);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new RefreshTokenResponse(
                accessToken.Token,
                accessToken.ExpiresAt,
                newRefreshToken.Token,
                newRefreshToken.ExpiresAt);

            return Result<RefreshTokenResponse>.Success(response);
        }
        catch (DomainException exception)
        {
            return Result<RefreshTokenResponse>.Failure(exception.Message);
        }
    }
}
