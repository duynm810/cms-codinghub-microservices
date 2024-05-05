using Category.API.Entities;
using Serilog;

namespace Category.API.Persistence;

public static class CategorySeedData
{
    public static async Task CategorySeedAsync(CategoryContext categoryDbContext)
    {
        if (!categoryDbContext.Categories.Any())
        {
            categoryDbContext.AddRange(GetCategories());
            await categoryDbContext.SaveChangesAsync();
            Log.Information("Seeded data for Category DB associated with context {DbContextName}",
                nameof(CategoryContext));
        }
    }
    
    private static IEnumerable<CategoryBase> GetCategories()
    {
        return new List<CategoryBase>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Home",
                Slug = "home",
                SeoDescription = "Welcome to Coding Hub, your ultimate guide to the world of programming and tech trends.",
                ParentId = Guid.Empty, // No parent
                SortOrder = 1,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Language Tutorials",
                Slug = "language-tutorials",
                SeoDescription = "Explore detailed tutorials on popular programming languages to boost your coding skills.",
                ParentId = Guid.Empty, // No parent
                SortOrder = 2,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Development",
                Slug = "development",
                SeoDescription = "Discover the latest development techniques and trends to stay ahead in the tech industry.",
                ParentId = Guid.Empty, // No parent
                SortOrder = 3,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "About Me",
                Slug = "about-me",
                SeoDescription = "Learn more about the face behind Coding Hub and my journey in the world of software development.",
                ParentId = Guid.Empty, // No parent
                SortOrder = 4,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Contact",
                Slug = "contact",
                SeoDescription = "Get in touch to discuss potential collaborations, or ask me anything about programming and tech.",
                ParentId = Guid.Empty, // No parent
                SortOrder = 4,
                IsActive = true
            }
        };
    }
}