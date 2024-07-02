using EventBus.IntegrationEvents.Interfaces;
using Hangfire.Api.Services.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Consumers.Posts;

public class PostSubmittedForApprovalEventConsumer(IBackgroundJobService backgroundJobService, ILogger logger)
    : IConsumer<IPostSubmittedForApprovalEvent>
{
    public async Task Consume(ConsumeContext<IPostSubmittedForApprovalEvent> context)
    {
        var message = context.Message;

        var jobId = backgroundJobService.SendEmail(message.To, message.Subject, message.EmailContent,
            message.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostSubmittedForApprovalEvent - Event Id: {EventId}, Job Id: {JobId}",
                message.Id, jobId);
        }
        else
        {
            logger.Warning("Failed to process PostSubmittedForApprovalEvent - Event Id: {EventId}", message.Id);
        }

        await Task.CompletedTask;
    }
}