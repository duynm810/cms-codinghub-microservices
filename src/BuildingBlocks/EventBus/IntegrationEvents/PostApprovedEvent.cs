using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostApprovedEvent(string SourceService) : IntegrationEvent(SourceService), IPostApprovedEvent
{
    public required string To { get; set; }

    public required string Subject { get; set; }

    public required string EmailContent { get; set; }

    public DateTimeOffset EnqueueAt { get; set; }
}