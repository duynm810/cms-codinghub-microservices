using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Enums;

namespace Post.Infrastructure.Persistence;

public class PostSeedData(PostContext context, ILogger logger)
    : IDatabaseSeeder
{
    private static readonly Random Random = new();

    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            await context.SaveChangesAsync();

            logger.Information("Data seeding completed successfully.");
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static DateTimeOffset RandomDate()
    {
        var range = (DateTime.UtcNow.Date - DateTime.UtcNow.AddYears(-2).Date).Days;
        return DateTime.UtcNow.AddDays(-Random.Next(range));
    }

    private async Task TrySeedAsync()
    {
        if (!context.Posts.Any())
        {
            var posts = new List<PostBase>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kinh nghiệm làm việc với .NET",
                    Slug = "kinh-nghiem-lam-viec-voi-net",
                    Content = "Chia sẻ kinh nghiệm làm việc với .NET...",
                    Summary = "Kinh nghiệm thực tế khi sử dụng .NET trong dự án...",
                    SeoDescription = "Kinh nghiệm làm việc với .NET...",
                    ViewCount = 100,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 10,
                    LikeCount = 50,
                    IsPinned = false,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kiến trúc microservices",
                    Slug = "kien-truc-microservices",
                    Content = "Các mẫu kiến trúc microservices...",
                    Summary = "Kiến trúc hệ thống với microservices...",
                    SeoDescription = "Kiến trúc microservices...",
                    ViewCount = 150,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 15,
                    LikeCount = 60,
                    IsPinned = true,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Thủ thuật sử dụng Angular",
                    Slug = "thu-thuat-su-dung-angular",
                    Content = "Các thủ thuật hữu ích khi làm việc với Angular...",
                    Summary = "Thủ thuật Angular giúp tăng hiệu suất...",
                    SeoDescription = "Thủ thuật sử dụng Angular...",
                    ViewCount = 200,
                    IsPaid = true,
                    RoyaltyAmount = 50,
                    PaidDate = RandomDate(),
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 4,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 20,
                    LikeCount = 70,
                    IsPinned = false,
                    IsFeatured = false
                }
            };

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();

            logger.Information("Seeded data for Post database associated with _context {DbContextName}",
                nameof(PostContext));
        }
    }
}