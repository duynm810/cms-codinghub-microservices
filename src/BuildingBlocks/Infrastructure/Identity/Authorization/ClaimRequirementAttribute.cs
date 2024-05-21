using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementAttribute : TypeFilterAttribute
{
    public ClaimRequirementAttribute(FunctionCodeEnum function, CommandCodeEnum command)
        : base(typeof(ClaimRequirementFilter))
    {
        Arguments = [function, command];
    }
}