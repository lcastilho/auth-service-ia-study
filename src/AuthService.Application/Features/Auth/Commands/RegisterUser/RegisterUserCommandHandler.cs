using AuthService.Application.Common;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Security;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result<RegisterUserResponse>.Failure("Email is already in use.");
        }

        try
        {
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Create(request.Name, request.Email, passwordHash, _dateTimeProvider.UtcNow);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new RegisterUserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.IsActive,
                user.CreatedAt);

            return Result<RegisterUserResponse>.Success(response);
        }
        catch (DomainException exception)
        {
            return Result<RegisterUserResponse>.Failure(exception.Message);
        }
    }
}
