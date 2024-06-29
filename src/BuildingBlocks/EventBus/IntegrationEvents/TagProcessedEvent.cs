using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record TagProcessedEvent(string SourceService) : IntegrationEvent(SourceService), ITagProcessedEvent
{
    public Guid PostId { get; set; }

    public List<Guid> TagIds { get; set; } = [];
}