using Contracts.Services.Interfaces;
using EventBus.IntergrationEvents;
using MassTransit;
using Post.Domain.Services;

namespace Post.Infrastructure.Services;

public class PostEmailTemplateService(IPublishEndpoint publishEndpoint, IEmailTemplateService emailTemplateService)
    : IPostEmailTemplateService
{
    public async Task SendApprovedPostEmail(Guid postId, string name, string? content, string? description)
    {
        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-approved");

        var emailContent = emailTemplate.Replace("[name]", name)
            .Replace("[content]", content)
            .Replace("[description]", description)
            .Replace("[postUrl]", $"https://yourwebsite.com/posts/{postId}");

        // Publish SendEmailEvent event
        var postApprovedEvent = new PostApprovedEvent
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been approved",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // hoặc thời điểm bạn muốn enqueue
        };

        await publishEndpoint.Publish(postApprovedEvent);
    }
}