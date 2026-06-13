using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}
