using Microsoft.EntityFrameworkCore;
using PostInSeries.Api.Persistence;

namespace PostInSeries.Api.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var seriesContext = scope.ServiceProvider.GetRequiredService<PostInSeriesContext>();
        seriesContext.Database.MigrateAsync().GetAwaiter().GetResult();

        return host;
    }
}