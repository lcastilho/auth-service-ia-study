using AuthService.Domain.Exceptions;

namespace AuthService.Domain.Entities;

public sealed class Role
{
    private readonly List<Permission> _permissions = [];

    private Role()
    {
        Name = string.Empty;
    }

    private Role(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public static Role Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Role name is required.");
        }

        return new Role(Guid.NewGuid(), name.Trim(), NormalizeOptionalText(description));
    }

    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (_permissions.Any(existingPermission => existingPermission.Id == permission.Id))
        {
            return;
        }

        _permissions.Add(permission);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
