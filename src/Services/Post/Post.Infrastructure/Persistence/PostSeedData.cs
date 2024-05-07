using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Serilog;
using Shared.Enums;

namespace Post.Infrastructure.Persistence;

public class PostSeedData(PostContext context, ILogger logger)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsSqlServer())
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
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while seeding the database.");
            throw;
        }
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
                    Name = "Cách học lập trình hiệu quả",
                    Slug = "cach-hoc-lap-trinh-hieu-qua",
                    Content = "Nội dung chi tiết về các phương pháp học lập trình...",
                    Description = "Bài viết này nêu bật các kỹ thuật học lập trình hiệu quả cho người mới bắt đầu.",
                    Thumbnail = "url-to-thumbnail1.jpg",
                    SeoDescription = "Học lập trình hiệu quả với các phương pháp đã được kiểm chứng.",
                    Source = "Tech Magazine",
                    Tags = "programming, education",
                    ViewCount = 150,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.Draft,
                    CategoryId = Guid.NewGuid(),
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Xu hướng công nghệ mới nhất 2024",
                    Slug = "xu-huong-cong-nghe-moi-nhat-2024",
                    Content = "Bài viết phân tích các xu hướng công nghệ mới nhất trong năm 2024...",
                    Description = "Một cái nhìn sâu sắc về các xu hướng công nghệ đang dẫn đầu trong năm 2024.",
                    Thumbnail = "url-to-thumbnail2.jpg",
                    SeoDescription = "Cập nhật những xu hướng công nghệ mới nhất của năm 2024.",
                    Source = "Tech News",
                    Tags = "technology, trends",
                    ViewCount = 250,
                    IsPaid = true,
                    RoyaltyAmount = 100.00,
                    Status = PostStatusEnum.Published,
                    CategoryId = Guid.NewGuid(),
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.Now,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Lịch sử của ngôn ngữ lập trình Python",
                    Slug = "lich-su-cua-ngon-ngu-lap-trinh-python",
                    Content = "Từ sự ra đời của Python đến nay, ngôn ngữ này đã phát triển như thế nào...",
                    Description = "Tìm hiểu về lịch sử phát triển của ngôn ngữ lập trình Python.",
                    Thumbnail = "url-to-thumbnail3.jpg",
                    SeoDescription = "Lịch sử hào hùng của Python, một trong những ngôn ngữ lập trình phổ biến nhất.",
                    Source = "Programming Historians",
                    Tags = "Python, programming history",
                    ViewCount = 300,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.WaitingForApproval,
                    CategoryId = Guid.NewGuid(),
                    AuthorUserId = Guid.NewGuid()
                }
            };

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();
        }
    }
}