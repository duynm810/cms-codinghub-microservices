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
                Name = "Trang chủ",
                Slug = "home",
                SeoDescription = "Chào mừng bạn đến với trang chủ.",
                ParentId = null,
                Icon = "icon_house_alt",
                SortOrder = 1,
                IsActive = true,
                IsStaticPage = true
            },
            new()
            {
                Id = 2,
                Name = "Kiến trúc hệ thống",
                Slug = "kien-truc-he-thong",
                SeoDescription = "Kiến thức về kiến trúc hệ thống giúp bạn thiết kế và xây dựng phần mềm hiệu quả.",
                ParentId = null,
                Color = "text-warning",
                SortOrder = 2,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 3,
                Name = "Kinh nghiệm thực tế",
                Slug = "kinh-nghiem-thuc-te",
                SeoDescription = "Những kinh nghiệm thực tế trong quá trình làm việc và phát triển phần mềm.",
                ParentId = null,
                Color = "text-danger",
                SortOrder = 3,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 4,
                Name = "Thủ thuật",
                Slug = "thu-thuat",
                SeoDescription = "Thủ thuật hữu ích giúp bạn làm việc hiệu quả hơn với .NET và Angular.",
                ParentId = null,
                Color = "text-primary",
                SortOrder = 4,
                IsStaticPage = false,
                IsActive = true
            },
            new()
            {
                Id = 5,
                Name = "Thông tin",
                Slug = "about-me",
                SeoDescription = "Tìm hiểu thêm về tôi và hành trình của tôi trong thế giới phát triển phần mềm.",
                ParentId = null,
                SortOrder = 5,
                IsStaticPage = true,
                IsActive = true
            },
            new()
            {
                Id = 6,
                Name = "Liên hệ",
                Slug = "contact",
                SeoDescription = "Liên hệ để thảo luận về các cơ hội hợp tác hoặc hỏi tôi bất cứ điều gì về lập trình và công nghệ.",
                ParentId = null,
                SortOrder = 6,
                IsStaticPage = true,
                IsActive = true
            }
        };
    }
}