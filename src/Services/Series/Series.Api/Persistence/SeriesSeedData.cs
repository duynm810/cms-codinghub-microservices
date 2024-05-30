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
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction to ASP.NET Core",
                    Slug = "introduction-to-aspnet-core",
                    Description = "A comprehensive series about ASP.NET Core fundamentals.",
                    SeoDescription = "Learn ASP.NET Core from scratch with our detailed guides.",
                    Thumbnail = "https://example.com/thumbnails/aspnetcore.jpg",
                    Content = "This series covers all you need to know about ASP.NET Core.",
                    AuthorUserId = Guid.NewGuid(), // Assume an existing Guid for Author
                    SortOrder = 1,
                    IsActive = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Advanced Topics in ASP.NET Core",
                    Slug = "advanced-topics-in-aspnet-core",
                    Description = "Dive deep into advanced concepts of ASP.NET Core.",
                    SeoDescription = "Explore advanced ASP.NET Core topics with in-depth articles.",
                    Thumbnail = "https://example.com/thumbnails/advancedaspnetcore.jpg",
                    Content = "This series covers advanced topics in ASP.NET Core.",
                    AuthorUserId = Guid.NewGuid(), // Assume an existing Guid for Author
                    SortOrder = 2,
                    IsActive = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Web Development Basics",
                    Slug = "web-development-basics",
                    Description = "An essential guide to get started with web development.",
                    SeoDescription = "Start your web development journey with our fundamental guides.",
                    Thumbnail = "https://example.com/thumbnails/webdevbasics.jpg",
                    Content = "This series covers the basics of web development, including HTML, CSS, and JavaScript.",
                    AuthorUserId = Guid.NewGuid(), // Assume an existing Guid for Author
                    SortOrder = 3,
                    IsActive = true
                }
            };

            seriesContext.Series.AddRange(seriesData);
            seriesContext.SaveChanges();
        }
    }
}