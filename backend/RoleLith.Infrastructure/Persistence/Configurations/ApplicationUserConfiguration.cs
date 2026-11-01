using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoleLith.Infrastructure.Identity;

namespace RoleLith.Infrastructure.Persistence.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.NormalizedEmail).IsRequired();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
    }
}