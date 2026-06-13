namespace AuthService.Application.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
