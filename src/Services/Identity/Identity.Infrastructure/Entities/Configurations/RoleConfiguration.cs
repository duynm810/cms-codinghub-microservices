using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace Identity.Infrastructure.Entities.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole()
            {
                Name = UserRolesConsts.Administrator,
                NormalizedName = UserRolesConsts.Administrator.Normalize(),
                Id = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Name = UserRolesConsts.Author,
                NormalizedName = UserRolesConsts.Author.Normalize(),
                Id = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Name = UserRolesConsts.Reader,
                NormalizedName = UserRolesConsts.Reader.Normalize(),
                Id = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Name = UserRolesConsts.Subscriber,
                NormalizedName = UserRolesConsts.Subscriber.Normalize(),
                Id = Guid.NewGuid().ToString()
            });
    }
}