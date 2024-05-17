using EventBus.IntergrationEvents;
using Hangfire.Api.Services.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Consumers.Posts;

public class PostSubmittedForApprovalEventConsumer(IBackgroundJobService backgroundJobService, ILogger logger)
    : IConsumer<PostSubmittedForApprovalEvent>
{
    public async Task Consume(ConsumeContext<PostSubmittedForApprovalEvent> context)
    {
        var emailEvent = context.Message;

        var jobId = backgroundJobService.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.EmailContent,
            emailEvent.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostSubmittedForApprovalEvent - Event Id: {EventId}, Job Id: {JobId}", emailEvent.Id, jobId);
        }
        else
        {
            logger.Warning("Failed to process PostSubmittedForApprovalEvent - Event Id: {EventId}", emailEvent.Id);
        }

        await Task.CompletedTask;
    }
}