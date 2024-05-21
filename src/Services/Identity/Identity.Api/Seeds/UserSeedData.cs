using System.Security.Claims;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Settings;

namespace Identity.Api.Seeds;

public static class UserSeedData
{
    public static void EnsureSeedData(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddLogging();
        services.AddDbContext<IdentityContext>(opt => opt.UseSqlServer(databaseSettings.ConnectionString));
        services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        using var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        CreateUser(scope, "Duy", "Nguyen", "82 Vo Van Ngan street, Binh Tho ward, Thu Duc city",
            Guid.NewGuid().ToString(), "admin123",
            "Administrator", "duynguyen810@example.com");
    }

    private static void CreateUser(IServiceScope scope, string firstName, string lastName,
        string address, string id, string password, string role, string email)
    {
        var userManagement = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = userManagement.FindByNameAsync(email).Result;
        if (user != null)
        {
            return;
        }

        user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Address = address,
            EmailConfirmed = true,
            Id = id,
        };
        var result = userManagement.CreateAsync(user, password).Result;
        CheckResult(result);

        var addToRoleResult = userManagement.AddToRoleAsync(user, role).Result;
        CheckResult(addToRoleResult);

        result = userManagement.AddClaimsAsync(user, new Claim[]
        {
            new(SystemConsts.Claims.UserName, user.UserName),
            new(SystemConsts.Claims.FirstName, user.FirstName),
            new(SystemConsts.Claims.LastName, user.LastName),
            new(SystemConsts.Claims.Roles, role),
            new(JwtClaimTypes.Address, user.Address),
            new(JwtClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id),
        }).Result;
        CheckResult(result);
    }

    private static void CheckResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }
}