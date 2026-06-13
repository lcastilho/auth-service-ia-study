using System.Net.Mail;
using AuthService.Domain.Exceptions;

namespace AuthService.Domain.Entities;

public sealed class User
{
    private readonly List<Role> _roles = [];
    private readonly List<RefreshToken> _refreshTokens = [];

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    private User(Guid id, string name, string email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public bool CanAuthenticate => IsActive;

    public static User Create(string name, string email, string passwordHash, DateTimeOffset createdAt)
    {
        ValidateName(name);
        var normalizedEmail = NormalizeEmail(email);
        ValidatePasswordHash(passwordHash);

        return new User(Guid.NewGuid(), name.Trim(), normalizedEmail, passwordHash.Trim(), createdAt);
    }

    public void Activate(DateTimeOffset updatedAt)
    {
        IsActive = true;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void ChangeName(string name, DateTimeOffset updatedAt)
    {
        ValidateName(name);

        Name = name.Trim();
        UpdatedAt = updatedAt;
    }

    public void ChangePasswordHash(string passwordHash, DateTimeOffset updatedAt)
    {
        ValidatePasswordHash(passwordHash);

        PasswordHash = passwordHash.Trim();
        UpdatedAt = updatedAt;
    }

    public void AddRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Any(existingRole => existingRole.Id == role.Id))
        {
            return;
        }

        _roles.Add(role);
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        if (refreshToken.UserId != Id)
        {
            throw new DomainException("Refresh token must belong to the user.");
        }

        _refreshTokens.Add(refreshToken);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("User name is required.");
        }
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash is required.");
        }
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("User email is required.");
        }

        try
        {
            var mailAddress = new MailAddress(email.Trim());
            return mailAddress.Address.ToLowerInvariant();
        }
        catch (FormatException)
        {
            throw new DomainException("User email format is invalid.");
        }
    }
}
