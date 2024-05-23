using System.Security;
using System.Security.Claims;
using Shared.Constants;
using Shared.Extensions;

namespace Infrastructure.Helpers;

public static class RequestHelper
{
    /// <summary>
    /// Checks if the user can access a user.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> for the current request.</param>
    /// <param name="userId">The user id.</param>
    /// <returns>A <see cref="bool"/> whether the user can access the user.</returns>
    public static Guid GetUserId(ClaimsPrincipal claimsPrincipal, Guid? userId = null)
    {
        var authenticatedUserId = claimsPrincipal.GetUserId();

        // UserId not provided, fall back to authenticated user id.
        if (userId.IsNullOrEmpty())
        {
            return authenticatedUserId;
        }

        // User must be administrator to access another user's details.
        var isAdministrator = claimsPrincipal.IsInRole(UserRolesConsts.Administrator);
        if (userId.Value != authenticatedUserId && !isAdministrator)
        {
            throw new SecurityException("Forbidden: Access is denied.");
        }

        return userId.Value;
    }
}