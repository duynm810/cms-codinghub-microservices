using Contracts.Services.Interfaces;
using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
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

        // Publish event
        var postApprovedEvent = new PostApprovedEvent
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been approved",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        await publishEndpoint.Publish<IPostApprovedEvent>(postApprovedEvent);
    }

    public async Task SendPostSubmissionForApprovalEmail(Guid postId, string name)
    {
        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-submitted");

        var emailContent = emailTemplate
            .Replace("[name]", name)
            .Replace("[postUrl]", $"https://yourwebsite.com/posts/{postId}");

        // Publish event
        var postSubmittedForApprovalEvent = new PostSubmittedForApprovalEvent
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been approval",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        await publishEndpoint.Publish<IPostSubmittedForApprovalEvent>(postSubmittedForApprovalEvent);
    }

    public async Task SendPostRejectionEmail(string name, string? reason)
    {
        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-rejected");

        var emailContent = emailTemplate
            .Replace("[name]", name)
            .Replace("[reason]", reason);

        // Publish event
        var postRejectedWithReasonEvent = new PostRejectedWithReasonEvent
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been rejected",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        await publishEndpoint.Publish<IPostRejectedWithReasonEvent>(postRejectedWithReasonEvent);
    }
}