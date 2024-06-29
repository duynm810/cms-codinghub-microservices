using Contracts.Services.Interfaces;
using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using Post.Domain.Services;
using Serilog;
using Shared.Settings;

namespace Post.Infrastructure.Services;

public class PostEmailTemplateService(
    IPublishEndpoint publishEndpoint,
    IEmailTemplateService emailTemplateService,
    IOptions<EventBusSettings> eventBusSettings,
    ILogger logger)
    : IPostEmailTemplateService
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;

    public async Task SendApprovedPostEmail(Guid postId, string name, string? content, string? description)
    {
        const string methodName = nameof(SendApprovedPostEmail);

        var serviceName = _eventBusSettings.ServiceName;

        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-approved");

        var emailContent = emailTemplate.Replace("[name]", name)
            .Replace("[content]", content)
            .Replace("[description]", description)
            .Replace("[postUrl]", $"https://yourwebsite.com/posts/{postId}");

        // Publish event
        var postApprovedEvent = new PostApprovedEvent(serviceName)
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been approved",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);
        
        try
        {
            await publishEndpoint.Publish<IPostApprovedEvent>(postApprovedEvent);
            logger.Information(
                "END Publish {MethodName} successfully - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "ERROR while publishing {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
            throw;
        }
    }

    public async Task SendPostSubmissionForApprovalEmail(Guid postId, string name)
    {
        const string methodName = nameof(SendPostSubmissionForApprovalEmail);

        var serviceName = _eventBusSettings.ServiceName;

        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-submitted");

        var emailContent = emailTemplate
            .Replace("[name]", name)
            .Replace("[postUrl]", $"https://yourwebsite.com/posts/{postId}");

        // Publish event
        var postSubmittedForApprovalEvent = new PostSubmittedForApprovalEvent(serviceName)
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been approval",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);

        try
        {
            await publishEndpoint.Publish<IPostSubmittedForApprovalEvent>(postSubmittedForApprovalEvent);
            logger.Information(
                "END Publish {MethodName} successfully - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while publishing {MethodName} - PostId: {PostId}, SourceService: {SourceService}",
                methodName, postId, serviceName);
            throw;
        }
    }

    public async Task SendPostRejectionEmail(string name, string? reason)
    {
        const string methodName = nameof(SendPostRejectionEmail);

        var serviceName = _eventBusSettings.ServiceName;

        var emailTemplate = emailTemplateService.ReadEmailTemplate("post-rejected");

        var emailContent = emailTemplate
            .Replace("[name]", name)
            .Replace("[reason]", reason);

        // Publish event
        var postRejectedWithReasonEvent = new PostRejectedWithReasonEvent(serviceName)
        {
            To = "cranneiffirixu-5288@yopmail.com", // Thay bằng địa chỉ email thật
            Subject = "Your post has been rejected",
            EmailContent = emailContent,
            EnqueueAt = DateTimeOffset.UtcNow // Thời điểm bạn muốn enqueue
        };

        logger.Information("BEGIN Publish {MethodName} - SourceService: {SourceService}", methodName, serviceName);

        try
        {
            await publishEndpoint.Publish<IPostRejectedWithReasonEvent>(postRejectedWithReasonEvent);
            logger.Information("END Publish {MethodName} successfully - SourceService: {SourceService}", methodName,
                serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while publishing {MethodName} - SourceService: {SourceService}", methodName,
                serviceName);
            throw;
        }
    }
}