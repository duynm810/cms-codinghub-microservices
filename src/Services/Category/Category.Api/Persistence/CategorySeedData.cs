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
                Slug = "home",
                SeoDescription = "Welcome to the home page.",
                ParentId = null,
                Icon = "icon_house_alt",
                SortOrder = 1,
                IsActive = true,
                IsStaticPage = true,  
            },
            new()
            {
                Id = 2,
                Name = "Language Tutorials",
                Slug = "language-tutorials",
                SeoDescription = "Explore detailed tutorials on popular programming languages to boost your coding skills.",
                ParentId = null, // No parent
                Color = "text-danger",
                SortOrder = 2,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 3,
                Name = "Development",
                Slug = "development",
                SeoDescription = "Discover the latest development techniques and trends to stay ahead in the tech industry.",
                ParentId = null, // No parent
                Color = "text-warning",
                SortOrder = 3,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 4,
                Name = "About Me",
                Slug = "about-me",
                SeoDescription = "Learn more about the face behind Coding Hub and my journey in the world of software development.",
                ParentId = null, // No parent
                SortOrder = 4,
                IsStaticPage = true,
                IsActive = true
            },
            new()
            {
                Id = 5,
                Name = "Contact",
                Slug = "contact",
                SeoDescription = "Get in touch to discuss potential collaborations, or ask me anything about programming and tech.",
                ParentId = null, // No parent
                SortOrder = 5,
                IsStaticPage = true,
                IsActive = true
            }
        };
    }
}