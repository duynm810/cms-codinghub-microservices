using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Shared.Constants;

namespace Infrastructure.Extensions;

public static class HostingEnvironmentExtensions
{
    public static bool IsLocal(this IWebHostEnvironment environment)
    {
        return environment.IsEnvironment(EnvironmentConsts.Local);
    } 
}