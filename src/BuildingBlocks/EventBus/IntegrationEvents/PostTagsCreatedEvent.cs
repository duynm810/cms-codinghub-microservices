using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostTagsCreatedEvent(string SourceService) : IntegrationEvent(SourceService), IPostTagsCreatedEvent
{
    public Guid PostId { get; set; }

    public List<Guid> TagIds { get; set; } = [];
}