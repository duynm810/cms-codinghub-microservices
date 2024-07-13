using System.Security.Claims;
using Shared.Constants;
using Shared.Dtos.Identity.User;

namespace Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Get user id from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>User id.</returns>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = GetClaimValue(user, InternalClaimTypesConsts.Claims.UserId);
        return string.IsNullOrEmpty(value)
            ? default
            : Guid.Parse(value);
    }
    
    /// <summary>
    /// Get role list from claims
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static List<string> GetRoles(this ClaimsPrincipal user)
    {
        var roles = GetClaimValue(user, InternalClaimTypesConsts.Claims.Roles);
        return string.IsNullOrEmpty(roles) ? [] : roles.Split(';').ToList();
    }
    
    /// <summary>
    /// Get fullname from claims, fallback to lastname if fullname is not available.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Full name or last name of the user.</returns>
    public static string GetFullName(this ClaimsPrincipal user)
    {
        var fullName = GetClaimValue(user, InternalClaimTypesConsts.Claims.FullName);
        if (!string.IsNullOrEmpty(fullName))
        {
            return fullName;
        }

        var lastName = GetClaimValue(user, InternalClaimTypesConsts.Claims.LastName);
        return string.IsNullOrEmpty(lastName) ? "User" : lastName;
    }
    
    /// <summary>
    /// Get username from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Username of the user.</returns>
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return GetClaimValue(user, InternalClaimTypesConsts.Claims.UserName) ?? "UnknownUser";
    }
    
    public static CurrentUserDto GetCurrentUser(this ClaimsPrincipal user)
    {
        var userId = GetClaimValue(user, InternalClaimTypesConsts.Claims.UserId);
        var roles = GetClaimValue(user, InternalClaimTypesConsts.Claims.Roles);

        return new CurrentUserDto
        {
            UserId = string.IsNullOrEmpty(userId) ? default : Guid.Parse(userId),
            Roles = string.IsNullOrEmpty(roles) ? new List<string>() : roles.Split(';').ToList()
        };
    }

    /// <summary>
    /// Get device id from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Device id.</returns>
    public static string? GetDeviceId(this ClaimsPrincipal user)
        => GetClaimValue(user, InternalClaimTypesConsts.Claims.DeviceId);

    /// <summary>
    /// Get device from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Device.</returns>
    public static string? GetDevice(this ClaimsPrincipal user)
        => GetClaimValue(user, InternalClaimTypesConsts.Claims.Device);

    /// <summary>
    /// Get client from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Client.</returns>
    public static string? GetClient(this ClaimsPrincipal user)
        => GetClaimValue(user, InternalClaimTypesConsts.Claims.Client);

    /// <summary>
    /// Get version from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Version.</returns>
    public static string? GetVersion(this ClaimsPrincipal user)
        => GetClaimValue(user, InternalClaimTypesConsts.Claims.Version);

    /// <summary>
    /// Get token from claims.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>Token.</returns>
    public static string? GetToken(this ClaimsPrincipal user)
        => GetClaimValue(user, InternalClaimTypesConsts.Claims.Token);
    
    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal?.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role) ?? false;
    }

    /// <summary>
    /// Gets a flag specifying whether the request is using an api key.
    /// </summary>
    /// <param name="user">Current claims principal.</param>
    /// <returns>The flag specifying whether the request is using an api key.</returns>
    public static bool GetIsApiKey(this ClaimsPrincipal user)
    {
        var claimValue = GetClaimValue(user, InternalClaimTypesConsts.Claims.IsApiKey);
        return bool.TryParse(claimValue, out var parsedClaimValue)
               && parsedClaimValue;
    }

    private static string? GetClaimValue(in ClaimsPrincipal user, string name)
        => user.Claims.FirstOrDefault(claim => claim.Type.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;
}