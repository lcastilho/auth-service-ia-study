using AuthService.Application.Common;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Security;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Exceptions;
using MediatR;
using DomainRefreshToken = AuthService.Domain.Entities.RefreshToken;

namespace AuthService.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure("Invalid email or password.");
        }

        if (!user.CanAuthenticate)
        {
            return Result<LoginResponse>.Failure("User is inactive.");
        }

        try
        {
            var accessToken = _jwtTokenService.CreateAccessToken(user);
            var refreshTokenResult = _jwtTokenService.CreateRefreshToken();
            var refreshToken = DomainRefreshToken.Create(
                user.Id,
                refreshTokenResult.Token,
                refreshTokenResult.ExpiresAt,
                _dateTimeProvider.UtcNow);

            user.AddRefreshToken(refreshToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new LoginResponse(
                user.Id,
                user.Name,
                user.Email,
                accessToken.Token,
                accessToken.ExpiresAt,
                refreshToken.Token,
                refreshToken.ExpiresAt);

            return Result<LoginResponse>.Success(response);
        }
        catch (DomainException exception)
        {
            return Result<LoginResponse>.Failure(exception.Message);
        }
    }
}
