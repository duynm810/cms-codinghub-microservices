using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostTagsUpdatedEvent(string SourceService) : IntegrationEvent(SourceService), IPostTagsUpdatedEvent
{
    public Guid PostId { get; set; }

    public List<Guid> NewTagIds { get; set; } = [];

    public List<Guid> TagsToRemove { get; set; } = [];
}