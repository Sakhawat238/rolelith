namespace RoleLith.Core.Organizations.Domain;

public sealed class Organization
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Organization() { }

    public Organization(Guid id, string name, DateTimeOffset createdAt, bool active)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name.Trim();
        if (name.Length > 100)
        {
            throw new ArgumentException("Organization name cannot exceed 100 characters.", nameof(name));
        }

        Id = id;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        CreatedAt = createdAt.ToUniversalTime();
        IsActive = active;
    }
}