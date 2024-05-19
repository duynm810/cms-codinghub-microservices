using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Entities.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole()
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Id = Guid.NewGuid().ToString()
            },
            new()
            {
                Name = "Member",
                NormalizedName = "MEMBER",
                Id = Guid.NewGuid().ToString()
            });
    }
}