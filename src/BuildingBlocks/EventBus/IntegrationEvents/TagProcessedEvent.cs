using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record TagProcessedEvent : IntegrationEvent, ITagProcessedEvent
{
    public Guid PostId { get; set; }

    public List<Guid> TagIds { get; set; } = [];
}