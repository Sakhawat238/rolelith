namespace RoleLith.Core.Organizations.Domain;

public sealed class OrganizationMembership
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private OrganizationMembership() { }

    public OrganizationMembership(Guid tenantId, Guid userId, DateTimeOffset createdAt)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(tenantId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        TenantId = tenantId;
        UserId = userId;
        CreatedAt = createdAt.ToUniversalTime();
    }
}