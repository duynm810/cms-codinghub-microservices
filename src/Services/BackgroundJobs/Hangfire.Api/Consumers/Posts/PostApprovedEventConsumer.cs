using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using Hangfire.Api.Services.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Consumers.Posts;

public class PostApprovedEventConsumer(IBackgroundJobService backgroundJobService, ILogger logger)
    : IConsumer<IPostApprovedEvent>
{
    public async Task Consume(ConsumeContext<IPostApprovedEvent> context)
    {
        var message = context.Message;

        var jobId = backgroundJobService.SendEmail(message.To, message.Subject, message.EmailContent,
            message.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostApprovedEvent - Event Id: {EventId}, Job Id: {JobId}", message.Id,
                jobId);
        }
        else
        {
            logger.Warning("Failed to process PostApprovedEvent - Event Id: {EventId}", message.Id);
        }

        await Task.CompletedTask;
    }
}