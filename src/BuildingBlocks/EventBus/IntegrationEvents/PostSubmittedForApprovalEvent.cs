using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostSubmittedForApprovalEvent(string SourceService) : IntegrationEvent(SourceService), IPostSubmittedForApprovalEvent
{
    public required string To { get; set; }

    public required string Subject { get; set; }

    public required string EmailContent { get; set; }

    public DateTimeOffset EnqueueAt { get; set; }
}