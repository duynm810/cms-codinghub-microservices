using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Enums;

namespace Post.Infrastructure.Persistence;

public class PostSeedData(PostContext context, ILogger logger) : IDatabaseSeeder
{
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
                    Summary = "Bài viết này nêu bật các kỹ thuật học lập trình hiệu quả cho người mới bắt đầu.",
                    Thumbnail = "url-to-thumbnail1.jpg",
                    SeoDescription = "Học lập trình hiệu quả với các phương pháp đã được kiểm chứng.",
                    Source = "Tech Magazine",
                    Tags = "programming, education",
                    ViewCount = 150,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.Draft,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Xu hướng công nghệ mới nhất 2024",
                    Slug = "xu-huong-cong-nghe-moi-nhat-2024",
                    Content = "Bài viết phân tích các xu hướng công nghệ mới nhất trong năm 2024...",
                    Summary = "Một cái nhìn sâu sắc về các xu hướng công nghệ đang dẫn đầu trong năm 2024.",
                    Thumbnail = "url-to-thumbnail2.jpg",
                    SeoDescription = "Cập nhật những xu hướng công nghệ mới nhất của năm 2024.",
                    Source = "Tech News",
                    Tags = "technology, trends",
                    ViewCount = 250,
                    IsPaid = true,
                    RoyaltyAmount = 100.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Lịch sử của ngôn ngữ lập trình Python",
                    Slug = "lich-su-cua-ngon-ngu-lap-trinh-python",
                    Content = "Từ sự ra đời của Python đến nay, ngôn ngữ này đã phát triển như thế nào...",
                    Summary = "Tìm hiểu về lịch sử phát triển của ngôn ngữ lập trình Python.",
                    Thumbnail = "url-to-thumbnail3.jpg",
                    SeoDescription = "Lịch sử hào hùng của Python, một trong những ngôn ngữ lập trình phổ biến nhất.",
                    Source = "Programming Historians",
                    Tags = "Python, programming history",
                    ViewCount = 300,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.WaitingForApproval,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "10 Thủ thuật Debug hiệu quả cho lập trình viên",
                    Slug = "10-thu-thuat-debug-hieu-qua",
                    Content =
                        "Khám phá những thủ thuật debug giúp tiết kiệm thời gian và tăng hiệu suất công việc của lập trình viên...",
                    Summary = "Bài viết chia sẻ những thủ thuật debug cực kỳ hữu ích cho bất kỳ lập trình viên nào.",
                    Thumbnail = "url-to-thumbnail4.jpg",
                    SeoDescription =
                        "Khám phá các kỹ thuật debug mà mọi lập trình viên cần biết để tối ưu hóa quá trình phát triển phần mềm.",
                    Source = "Dev Tricks",
                    Tags = "debugging, software development",
                    ViewCount = 120,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Giới thiệu về Machine Learning",
                    Slug = "gioi-thieu-ve-machine-learning",
                    Content = "Một bước chân vào thế giới máy học: Từ lý thuyết đến ứng dụng...",
                    Summary =
                        "Tìm hiểu những khái niệm cơ bản về Machine Learning và cách thức nó đang thay đổi thế giới.",
                    Thumbnail = "url-to-thumbnail5.jpg",
                    SeoDescription = "Khám phá bản chất và tiềm năng của Machine Learning trong thời đại số.",
                    Source = "AI Insights",
                    Tags = "Machine Learning, AI",
                    ViewCount = 200,
                    IsPaid = true,
                    RoyaltyAmount = 50.0,
                    Status = PostStatusEnum.WaitingForApproval,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Blockchain và tương lai tài chính",
                    Slug = "blockchain-va-tuong-lai-tai-chinh",
                    Content = "Phân tích sâu về cách mà Blockchain đang làm thay đổi ngành tài chính toàn cầu...",
                    Summary = "Một cái nhìn chi tiết về Blockchain và các ứng dụng của nó trong ngành tài chính.",
                    Thumbnail = "url-to-thumbnail6.jpg",
                    SeoDescription =
                        "Tìm hiểu sự ảnh hưởng của Blockchain đối với ngành tài chính và những thách thức liên quan.",
                    Source = "Finance Tech",
                    Tags = "Blockchain, finance, technology",
                    ViewCount = 330,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.Draft,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Tiến bộ trong thực tế ảo VR",
                    Slug = "tien-bo-trong-thuc-te-ao-vr",
                    Content =
                        "Bài viết này đưa ra cái nhìn tổng quan về những tiến bộ gần đây trong công nghệ thực tế ảo VR...",
                    Summary =
                        "Khám phá những bước tiến vượt bậc trong công nghệ VR và tác động của nó đến ngành công nghiệp giải trí.",
                    Thumbnail = "url-to-thumbnail7.jpg",
                    SeoDescription =
                        "Cập nhật những phát triển mới nhất trong công nghệ thực tế ảo và những ảnh hưởng của nó đến cuộc sống hàng ngày.",
                    Source = "VR Today",
                    Tags = "VR, technology",
                    ViewCount = 140,
                    IsPaid = true,
                    RoyaltyAmount = 75.0,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sự phát triển của IoT trong ngành công nghiệp 4.0",
                    Slug = "su-phat-trien-cua-iot-trong-nganh-cong-nghiep-4-0",
                    Content = "Khám phá sự gắn kết của IoT với Công nghiệp 4.0 và những thay đổi mà nó mang lại...",
                    Summary =
                        "Tìm hiểu cách IoT đang thay đổi bối cảnh sản xuất công nghiệp và tác động của nó đến ngành công nghiệp.",
                    Thumbnail = "url-to-thumbnail8.jpg",
                    SeoDescription =
                        "Phân tích sự kết hợp của IoT trong Công nghiệp 4.0 và những cải tiến nó mang lại.",
                    Source = "Industry Updates",
                    Tags = "IoT, Industry 4.0",
                    ViewCount = 280,
                    IsPaid = false,
                    RoyaltyAmount = 0.0,
                    Status = PostStatusEnum.WaitingForApproval,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Lập trình web hiện đại với JavaScript ES6",
                    Slug = "lap-trinh-web-hien-dai-voi-javascript-es6",
                    Content = "Hướng dẫn từng bước cách sử dụng JavaScript ES6 để tạo ra các ứng dụng web hiện đại...",
                    Summary =
                        "Khám phá các tính năng của JavaScript ES6 và cách áp dụng chúng trong lập trình web hiện đại.",
                    Thumbnail = "url-to-thumbnail9.jpg",
                    SeoDescription = "Học cách lập trình web hiệu quả và hiện đại sử dụng JavaScript ES6.",
                    Source = "Web Dev Blog",
                    Tags = "JavaScript, web development",
                    ViewCount = 170,
                    IsPaid = true,
                    RoyaltyAmount = 85.0,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Hướng dẫn sử dụng Python cho người mới bắt đầu",
                    Slug = "huong-dan-su-dung-python-cho-nguoi-moi-bat-dau",
                    Content = "Bài viết cung cấp các bước cơ bản để bắt đầu học Python...",
                    Summary = "Một hướng dẫn chi tiết cho người mới bắt đầu học Python.",
                    Thumbnail = "url-to-thumbnail3.jpg",
                    SeoDescription = "Hướng dẫn từng bước để bắt đầu học Python.",
                    Source = "Code Academy",
                    Tags = "python, tutorial",
                    ViewCount = 150,
                    IsPaid = false,
                    RoyaltyAmount = 0.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Các mẫu thiết kế phần mềm quan trọng",
                    Slug = "cac-mau-thiet-ke-phan-mem-quan-trong",
                    Content = "Bài viết giới thiệu các mẫu thiết kế phần mềm cơ bản và cách sử dụng chúng...",
                    Summary = "Giới thiệu các mẫu thiết kế phần mềm quan trọng và ứng dụng của chúng.",
                    Thumbnail = "url-to-thumbnail4.jpg",
                    SeoDescription = "Tìm hiểu về các mẫu thiết kế phần mềm và ứng dụng thực tế.",
                    Source = "Dev Blog",
                    Tags = "software design, patterns",
                    ViewCount = 200,
                    IsPaid = true,
                    RoyaltyAmount = 80.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Phát triển ứng dụng web với ASP.NET Core",
                    Slug = "phat-trien-ung-dung-web-voi-asp-net-core",
                    Content = "Bài viết hướng dẫn phát triển ứng dụng web sử dụng ASP.NET Core...",
                    Summary = "Hướng dẫn chi tiết phát triển ứng dụng web với ASP.NET Core.",
                    Thumbnail = "url-to-thumbnail5.jpg",
                    SeoDescription = "Học cách phát triển ứng dụng web với ASP.NET Core.",
                    Source = "Tech Tutorials",
                    Tags = "asp.net core, web development",
                    ViewCount = 300,
                    IsPaid = false,
                    RoyaltyAmount = 0.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Những cải tiến mới trong JavaScript năm 2024",
                    Slug = "nhung-cai-tien-moi-trong-javascript-nam-2024",
                    Content = "Bài viết khám phá các tính năng mới được thêm vào JavaScript trong năm 2024...",
                    Summary = "Tìm hiểu những cải tiến mới nhất trong JavaScript năm 2024.",
                    Thumbnail = "url-to-thumbnail6.jpg",
                    SeoDescription = "Khám phá những tính năng mới trong JavaScript năm 2024.",
                    Source = "JS World",
                    Tags = "javascript, new features",
                    ViewCount = 220,
                    IsPaid = true,
                    RoyaltyAmount = 95.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Cách tối ưu hóa hiệu suất ứng dụng di động",
                    Slug = "cach-toi-uu-hoa-hieu-suat-ung-dung-di-dong",
                    Content = "Bài viết hướng dẫn cách tối ưu hóa hiệu suất cho các ứng dụng di động...",
                    Summary = "Các chiến lược tối ưu hóa hiệu suất ứng dụng di động.",
                    Thumbnail = "url-to-thumbnail7.jpg",
                    SeoDescription = "Tối ưu hóa hiệu suất cho các ứng dụng di động.",
                    Source = "Mobile Dev",
                    Tags = "mobile, performance optimization",
                    ViewCount = 180,
                    IsPaid = false,
                    RoyaltyAmount = 0.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Phân tích dữ liệu với SQL và Python",
                    Slug = "phan-tich-du-lieu-voi-sql-va-python",
                    Content = "Bài viết cung cấp các kỹ thuật phân tích dữ liệu sử dụng SQL và Python...",
                    Summary = "Hướng dẫn phân tích dữ liệu sử dụng SQL và Python.",
                    Thumbnail = "url-to-thumbnail8.jpg",
                    SeoDescription = "Phân tích dữ liệu chuyên sâu với SQL và Python.",
                    Source = "Data Science Blog",
                    Tags = "data analysis, SQL, Python",
                    ViewCount = 275,
                    IsPaid = true,
                    RoyaltyAmount = 120.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 2,
                    AuthorUserId = Guid.NewGuid(),
                    PaidDate = DateTime.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Xây dựng hệ thống microservices với Docker và Kubernetes",
                    Slug = "xay-dung-he-thong-microservices-voi-docker-va-kubernetes",
                    Content =
                        "Bài viết hướng dẫn cách xây dựng và quản lý hệ thống microservices sử dụng Docker và Kubernetes...",
                    Summary = "Hướng dẫn xây dựng hệ thống microservices với Docker và Kubernetes.",
                    Thumbnail = "url-to-thumbnail9.jpg",
                    SeoDescription = "Xây dựng và quản lý hệ thống microservices với Docker và Kubernetes.",
                    Source = "DevOps Blog",
                    Tags = "microservices, Docker, Kubernetes",
                    ViewCount = 350,
                    IsPaid = false,
                    RoyaltyAmount = 0.00,
                    Status = PostStatusEnum.Published,
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = 3,
                    AuthorUserId = Guid.NewGuid()
                }
            };

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();

            logger.Information("Seeded data for Post database associated with context {DbContextName}",
                nameof(PostContext));
        }
    }
}