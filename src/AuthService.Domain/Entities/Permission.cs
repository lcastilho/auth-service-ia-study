using AuthService.Domain.Exceptions;

namespace AuthService.Domain.Entities;

public sealed class Permission
{
    private Permission()
    {
        Name = string.Empty;
    }

    private Permission(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    public static Permission Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Permission name is required.");
        }

        return new Permission(Guid.NewGuid(), name.Trim(), NormalizeOptionalText(description));
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
