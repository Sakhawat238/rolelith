using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoleLith.Infrastructure.Identity;
using RoleLith.Infrastructure.Persistence;

namespace RoleLith.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityPersistence(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.AddDbContext<RoleLithDbContext>(options => options.UseNpgsql(connectionString));
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddEntityFrameworkStores<RoleLithDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}