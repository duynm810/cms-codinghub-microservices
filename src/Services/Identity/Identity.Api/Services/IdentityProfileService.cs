using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Identity.Api.Services.Intefaces;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;

namespace Identity.Api.Services;

public class IdentityProfileService(IUserClaimsPrincipalFactory<User> claimsFactory, UserManager<User> userManager)
    : IIdentityProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await userManager.FindByIdAsync(sub);
        if (user == null)
        {
            throw new ArgumentNullException($"User Id not found!");
        }

        var principal = await claimsFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();
        var roles = await userManager.GetRolesAsync(user);

        if (user.FirstName != null)
        {
            claims.Add(new Claim(SystemConsts.Claims.FirstName, user.FirstName));
        }

        if (user.LastName != null)
        {
            claims.Add(new Claim(SystemConsts.Claims.LastName, user.LastName));
        }

        if (user.UserName != null)
        {
            claims.Add(new Claim(SystemConsts.Claims.UserName, user.UserName));
            claims.Add(new Claim(SystemConsts.Claims.UserId, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        if (user.Email != null)
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim(SystemConsts.Claims.Roles, string.Join(";", roles)));

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await userManager.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}