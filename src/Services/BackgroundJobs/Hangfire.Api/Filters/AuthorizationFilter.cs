using Hangfire.Dashboard;

namespace Hangfire.Api.Filters;

public class AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}