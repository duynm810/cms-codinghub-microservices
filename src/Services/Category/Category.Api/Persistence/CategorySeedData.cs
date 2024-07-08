using Category.Api.Entities;
using ILogger = Serilog.ILogger;

namespace Category.Api.Persistence;

public static class CategorySeedData
{
    public static async Task CategorySeedAsync(CategoryContext categoryDbContext, ILogger logger)
    {
        if (!categoryDbContext.Categories.Any())
        {
            categoryDbContext.AddRange(GetCategories());
            await categoryDbContext.SaveChangesAsync();
            logger.Information("Seeded data for Category database associated with context {DbContextName}",
                nameof(CategoryContext));
        }
    }

    private static IEnumerable<CategoryBase> GetCategories()
    {
        return new List<CategoryBase>
        {
            new()
            {
                Id = 1,
                Name = "Home",
                Slug = "dashboard",
                SeoDescription = "Welcome to the home page.",
                ParentId = null,
                Icon = "icon_house_alt",
                SortOrder = 1,
                IsActive = true,
                IsStaticPage = true
            },
            new()
            {
                Id = 2,
                Name = "News",
                Slug = "news",
                SeoDescription = "Knowledge about system architecture helps you design and build software efficiently.",
                ParentId = null,
                Color = "text-warning",
                SortOrder = 2,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 3,
                Name = "Experience",
                Slug = "experience",
                SeoDescription = "Practical experiences in the process of working and developing software.",
                ParentId = null,
                Color = "text-danger",
                SortOrder = 3,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 4,
                Name = "Tips and Tricks",
                Slug = "tips-and-tricks",
                SeoDescription = "Useful tips to help you work more efficiently with .NET and Angular.",
                ParentId = null,
                Color = "text-primary",
                SortOrder = 4,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 5,
                Name = "About Me",
                Slug = "about-me",
                SeoDescription = "Learn more about me and my journey in the world of software development.",
                ParentId = null,
                SortOrder = 5,
                IsStaticPage = true,
                IsActive = true
            }
        };
    }
}