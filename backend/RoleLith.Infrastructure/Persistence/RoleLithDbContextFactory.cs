using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoleLith.Infrastructure.Persistence;

public sealed class RoleLithDbContextFactory : IDesignTimeDbContextFactory<RoleLithDbContext>
{
    public RoleLithDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgresql");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Set ConnectionStrings__Postgresql before running EF tools.");
        }

        return new RoleLithDbContext(new DbContextOptionsBuilder<RoleLithDbContext>()
            .UseNpgsql(connectionString).Options);
    }
}