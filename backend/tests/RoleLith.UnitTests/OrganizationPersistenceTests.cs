using Microsoft.EntityFrameworkCore;
using RoleLith.Core.Organizations.Domain;
using RoleLith.Infrastructure.Persistence;

namespace RoleLith.UnitTests;

public class OrganizationPersistenceTests
{
    private static RoleLithDbContext CreateContext(Guid? tenantId = null) => new(
        new DbContextOptionsBuilder<RoleLithDbContext>()
            .UseNpgsql("Host=localhost;Database=model_tests;Username=unused").Options, tenantId);

    [Fact]
    public void Organization_normalizes_name_and_creation_time()
    {
        var time = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(6));
        var organization = new Organization(Guid.NewGuid(), "  Example Ltd  ", time, true);
        Assert.Equal("Example Ltd", organization.Name);
        Assert.Equal("EXAMPLE LTD", organization.NormalizedName);
        Assert.Equal(TimeSpan.Zero, organization.CreatedAt.Offset);
        Assert.Equal(time, organization.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Empty_name_is_rejected(string name) =>
        Assert.ThrowsAny<ArgumentException>(() => new Organization(Guid.NewGuid(), name, DateTimeOffset.UtcNow, true));

    [Fact]
    public void Organization_names_are_unique_and_membership_identity_includes_tenant()
    {
        using var context = CreateContext();
        var organization = context.Model.FindEntityType(typeof(Organization))!;
        Assert.Contains(organization.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(x => x.Name).SequenceEqual([nameof(Organization.NormalizedName)]));
        var membership = context.Model.FindEntityType(typeof(OrganizationMembership))!;
        Assert.Equal(["TenantId", "UserId"], membership.FindPrimaryKey()!.Properties.Select(x => x.Name));
        Assert.All(membership.GetForeignKeys(), key => Assert.Equal(DeleteBehavior.Restrict, key.DeleteBehavior));
    }

    [Fact]
    public void Query_filters_fail_closed_and_generate_tenant_scoped_SQL()
    {
        using var missing = CreateContext();
        using var selected = CreateContext(Guid.NewGuid());
        var missingSql = missing.OrganizationMemberships.ToQueryString();
        var selectedSql = selected.OrganizationMemberships.ToQueryString();
        Assert.Contains("WHERE", missingSql);
        Assert.Contains("tenant_id", selectedSql);
        Assert.Contains("WHERE", selectedSql);
        Assert.NotEqual(missingSql, selectedSql);
    }

    [Fact]
    public async Task Missing_tenant_rejects_creation_before_database_access()
    {
        using var context = CreateContext();
        context.Organizations.Add(new Organization(Guid.NewGuid(), "Example", DateTimeOffset.UtcNow, true));
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }

    [Theory]
    [InlineData(EntityState.Added)]
    [InlineData(EntityState.Modified)]
    [InlineData(EntityState.Deleted)]
    public async Task Cross_tenant_writes_are_rejected_before_database_access(EntityState state)
    {
        using var context = CreateContext(Guid.NewGuid());
        var membership = new OrganizationMembership(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        context.Entry(membership).State = state;
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }
}
