using Identity.Api.Entities;
using Identity.Api.Entities.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Persistence;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // co 2 cach code de apply configuration
        //builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        builder.ApplyIdentityConfiguration();
    }
}