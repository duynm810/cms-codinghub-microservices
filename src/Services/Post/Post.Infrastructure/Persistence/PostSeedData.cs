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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Tối ưu hóa hiệu suất .NET",
                    Slug = "toi-uu-hoa-hieu-suat-net",
                    Content = "Các cách tối ưu hóa hiệu suất khi sử dụng .NET...",
                    Summary = "Tối ưu hóa .NET để cải thiện hiệu suất hệ thống...",
                    SeoDescription = "Tối ưu hóa hiệu suất .NET...",
                    ViewCount = 120,
                    IsPaid = true,
                    RoyaltyAmount = 75,
                    PaidDate = RandomDate(),
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 12,
                    LikeCount = 55,
                    IsPinned = true,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Xây dựng hệ thống chịu tải cao",
                    Slug = "xay-dung-he-thong-chiu-tai-cao",
                    Content = "Hướng dẫn xây dựng hệ thống chịu tải cao...",
                    Summary = "Các phương pháp và công cụ để xây dựng hệ thống chịu tải cao...",
                    SeoDescription = "Xây dựng hệ thống chịu tải cao...",
                    ViewCount = 180,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 18,
                    LikeCount = 65,
                    IsPinned = false,
                    IsFeatured = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kinh nghiệm phát triển ứng dụng di động",
                    Slug = "kinh-nghiem-phat-trien-ung-dung-di-dong",
                    Content = "Chia sẻ kinh nghiệm phát triển ứng dụng di động...",
                    Summary = "Các kinh nghiệm quý báu khi phát triển ứng dụng di động...",
                    SeoDescription = "Kinh nghiệm phát triển ứng dụng di động...",
                    ViewCount = 220,
                    IsPaid = false,
                    Status = PostStatusEnum.Draft,
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 22,
                    LikeCount = 80,
                    IsPinned = true,
                    IsFeatured = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Thủ thuật tối ưu hóa ReactJS",
                    Slug = "thu-thuat-toi-uu-hoa-reactjs",
                    Content = "Các thủ thuật giúp tối ưu hóa hiệu suất của ReactJS...",
                    Summary = "Tối ưu hóa ReactJS để tăng tốc độ tải và hiệu suất...",
                    SeoDescription = "Thủ thuật tối ưu hóa ReactJS...",
                    ViewCount = 160,
                    IsPaid = true,
                    RoyaltyAmount = 60,
                    PaidDate = RandomDate(),
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 4,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 16,
                    LikeCount = 75,
                    IsPinned = false,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kiến trúc serverless",
                    Slug = "kien-truc-serverless",
                    Content = "Giới thiệu về kiến trúc serverless và các ứng dụng...",
                    Summary = "Lợi ích và thách thức của kiến trúc serverless...",
                    SeoDescription = "Kiến trúc serverless...",
                    ViewCount = 110,
                    IsPaid = false,
                    Status = PostStatusEnum.WaitingForApproval,
                    CreatedDate = RandomDate(),
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 11,
                    LikeCount = 45,
                    IsPinned = false,
                    IsFeatured = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kinh nghiệm học lập trình",
                    Slug = "kinh-nghiem-hoc-lap-trinh",
                    Content = "Chia sẻ kinh nghiệm và phương pháp học lập trình hiệu quả...",
                    Summary = "Các mẹo và chiến lược học lập trình nhanh chóng...",
                    SeoDescription = "Kinh nghiệm học lập trình...",
                    ViewCount = 130,
                    IsPaid = false,
                    Status = PostStatusEnum.Rejected,
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 13,
                    LikeCount = 35,
                    IsPinned = false,
                    IsFeatured = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Thủ thuật sử dụng Docker",
                    Slug = "thu-thuat-su-dung-docker",
                    Content = "Các thủ thuật và mẹo khi làm việc với Docker...",
                    Summary = "Tối ưu hóa Docker để quản lý container hiệu quả...",
                    SeoDescription = "Thủ thuật sử dụng Docker...",
                    ViewCount = 190,
                    IsPaid = true,
                    RoyaltyAmount = 80,
                    PaidDate = RandomDate(),
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 4,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 19,
                    LikeCount = 70,
                    IsPinned = true,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Phát triển ứng dụng web với .NET và Angular",
                    Slug = "phat-trien-ung-dung-web-voi-net-va-angular",
                    Content = "Hướng dẫn phát triển ứng dụng web sử dụng .NET và Angular...",
                    Summary = "Các bước và kinh nghiệm trong việc phát triển ứng dụng web với .NET và Angular...",
                    SeoDescription = "Phát triển ứng dụng web với .NET và Angular...",
                    ViewCount = 240,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 24,
                    LikeCount = 85,
                    IsPinned = true,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Thiết kế API RESTful hiệu quả",
                    Slug = "thiet-ke-api-restful-hieu-qua",
                    Content = "Các nguyên tắc và hướng dẫn để thiết kế API RESTful hiệu quả...",
                    Summary = "Tìm hiểu về cách thiết kế API RESTful để đảm bảo tính dễ dùng và hiệu quả...",
                    SeoDescription = "Thiết kế API RESTful hiệu quả...",
                    ViewCount = 210,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 21,
                    LikeCount = 75,
                    IsPinned = false,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Thủ thuật nâng cao hiệu suất hệ thống",
                    Slug = "thu-thuat-nang-cao-hieu-suat-he-thong",
                    Content = "Các thủ thuật giúp nâng cao hiệu suất hệ thống...",
                    Summary = "Các biện pháp và kỹ thuật để tối ưu hóa hiệu suất hệ thống...",
                    SeoDescription = "Thủ thuật nâng cao hiệu suất hệ thống...",
                    ViewCount = 220,
                    IsPaid = true,
                    RoyaltyAmount = 90,
                    PaidDate = RandomDate(),
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 4,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 22,
                    LikeCount = 80,
                    IsPinned = true,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kinh nghiệm phát triển dự án Agile",
                    Slug = "kinh-nghiem-phat-trien-du-an-agile",
                    Content = "Chia sẻ kinh nghiệm và phương pháp phát triển dự án theo Agile...",
                    Summary = "Các bước và chiến lược để phát triển dự án thành công theo Agile...",
                    SeoDescription = "Kinh nghiệm phát triển dự án Agile...",
                    ViewCount = 140,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 14,
                    LikeCount = 50,
                    IsPinned = false,
                    IsFeatured = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Kiến trúc micro frontends",
                    Slug = "kien-truc-micro-frontends",
                    Content = "Giới thiệu về kiến trúc micro frontends và cách triển khai...",
                    Summary = "Lợi ích và thách thức của kiến trúc micro frontends...",
                    SeoDescription = "Kiến trúc micro frontends...",
                    ViewCount = 170,
                    IsPaid = false,
                    Status = PostStatusEnum.Published,
                    PublishedDate = RandomDate(),
                    CreatedDate = RandomDate(),
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    CommentCount = 17,
                    LikeCount = 65,
                    IsPinned = true,
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