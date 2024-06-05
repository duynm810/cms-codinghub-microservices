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
        return new List<TagBase>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tech"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Programming"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tutorial"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Microservices"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "CSharp"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Language Programming"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Trick"
            }
        };
    }
}