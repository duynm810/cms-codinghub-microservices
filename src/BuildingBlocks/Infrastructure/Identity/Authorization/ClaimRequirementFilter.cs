using System.Text.Json;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Constants;
using Shared.Enums;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementFilter(FunctionCodeEnum functionCode, CommandCodeEnum commandCode) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionsClaim = context.HttpContext.User.Claims
            .SingleOrDefault(c => c.Type.Equals(InternalClaimTypesConsts.Claims.Permissions));
        if (permissionsClaim != null)
        {
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);
            if (permissions != null &&
                !permissions.Contains(PermissionHelper.GetPermission(functionCode, commandCode)))
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            context.Result = new ForbidResult();
        }
    }
}