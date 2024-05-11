using Microsoft.EntityFrameworkCore;
using Series.Api.Entities;

namespace Series.Api.Persistence;

public static class SeriesSeedData
{
    public static IHost SeedData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var seriesContext = scope.ServiceProvider.GetRequiredService<SeriesContext>();
        seriesContext.Database.MigrateAsync().GetAwaiter().GetResult();

        CreateSeriesData(seriesContext);

        return host;
    }

    private static void CreateSeriesData(SeriesContext seriesContext)
    {
        // Check if data already exists to avoid duplicate seeding
        if (!seriesContext.Series.Any())
        {
            var seriesData = new List<SeriesBase>
            {
                new SeriesBase
                {
                    Id = Guid.NewGuid(),
                    Name = "Introduction to ASP.NET Core",
                    Slug = "introduction-to-aspnet-core",
                    Description = "A comprehensive series about ASP.NET Core fundamentals.",
                    SeoDescription = "Learn ASP.NET Core from scratch with our detailed guides.",
                    Thumbnail = "https://example.com/thumbnails/aspnetcore.jpg",
                    Content = "This series covers all you need to know about ASP.NET Core.",
                    AuthorUserId = Guid.NewGuid(), // Assume an existing Guid for Author
                    SortOrder = 1,
                    IsActive = true
                },
                new SeriesBase
                {
                    Id = Guid.NewGuid(),
                    Name = "Advanced Topics in ASP.NET Core",
                    Slug = "advanced-topics-in-aspnet-core",
                    Description = "Dive deep into advanced concepts of ASP.NET Core.",
                    SeoDescription = "Explore advanced ASP.NET Core topics with in-depth articles.",
                    Thumbnail = "https://example.com/thumbnails/advancedaspnetcore.jpg",
                    Content = "This series covers advanced topics in ASP.NET Core.",
                    AuthorUserId = Guid.NewGuid(), // Assume an existing Guid for Author
                    SortOrder = 2,
                    IsActive = true
                }
            };

            seriesContext.Series.AddRange(seriesData);
            seriesContext.SaveChanges();
        }
    }
}