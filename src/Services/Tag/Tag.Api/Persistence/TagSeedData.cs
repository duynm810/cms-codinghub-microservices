using Tag.Api.Entities;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Persistence;

public static class TagSeedData
{
    public static async Task TagSeedAsync(TagContext tagDbContext, ILogger logger)
    {
        if (!tagDbContext.Tags.Any())
        {
            tagDbContext.AddRange(GetTags());
            await tagDbContext.SaveChangesAsync();
            logger.Information("Seeded data for Tag database associated with context {DbContextName}",
                nameof(TagContext));
        }
    }

    private static IEnumerable<TagBase> GetTags()
    {
        var random = new Random();
        
        return new List<TagBase>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "tech",
                Slug = "tech",
                UsageCount = random.Next(1, 21) // Random value between 1 to 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "programming",
                Slug = "programming",
                UsageCount = random.Next(1, 21)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tutorial",
                Slug = "tutorial",
                UsageCount = random.Next(1, 21)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Microservices",
                Slug = "microservices",
                UsageCount = random.Next(1, 21)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "CSharp",
                Slug = "csharp",
                UsageCount = random.Next(1, 21)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "programming",
                Slug = "programming",
                UsageCount = random.Next(1, 21)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "trick",
                Slug = "trick",
                UsageCount = random.Next(1, 21)
            }
        };
    }
}