using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RoleLith.Core.Organizations.Domain;
using RoleLith.Infrastructure.Identity;
using System.Text.RegularExpressions;

namespace RoleLith.Infrastructure.Persistence;

public sealed class RoleLithDbContext(DbContextOptions<RoleLithDbContext> options, 
    Guid? tenantId = null) : IdentityUserContext<ApplicationUser, Guid>(options)
{
    private readonly Guid? _tenantId = tenantId;

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(RoleLithDbContext).Assembly);
        ApplySnakeCaseNamingConvention(builder);
        builder.Entity<Organization>().HasQueryFilter(x => _tenantId != null && x.Id == _tenantId);
        builder.Entity<OrganizationMembership>().HasQueryFilter(x => _tenantId != null && x.TenantId == _tenantId);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ValidateTenantWrites();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ValidateTenantWrites();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ValidateTenantWrites()
    {
        ChangeTracker.DetectChanges();
        foreach (var entry in ChangeTracker.Entries().Where(x =>
                     x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            Guid? entityTenantId = entry.Entity switch
            {
                Organization organization => organization.Id,
                OrganizationMembership membership => membership.TenantId,
                _ => null
            };

            if (entityTenantId is null) continue;
            if (_tenantId is null || _tenantId == Guid.Empty || entityTenantId != _tenantId)
            {
                throw new InvalidOperationException("A matching organization context is required for this write.");
            }
        }
    }

    private static void ApplySnakeCaseNamingConvention(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            if (entity.GetTableName() != null)
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));
            }
            
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName(StoreObjectIdentifier.Table(property.DeclaringType!.GetTableName()!, null));
                property.SetColumnName(ToSnakeCase(columnName!));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()!));
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(ToSnakeCase(key.GetConstraintName()!));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}