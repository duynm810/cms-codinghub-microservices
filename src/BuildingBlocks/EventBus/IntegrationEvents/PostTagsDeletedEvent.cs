using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostTagsDeletedEvent(string SourceService) : IntegrationEvent(SourceService), IPostTagsDeletedEvent
{
    public Guid PostId { get; set; }
}