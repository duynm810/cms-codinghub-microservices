using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;

namespace EventBus.IntegrationEvents;

public record PostDeletedEvent(string SourceService) : IntegrationEvent(SourceService), IPostDeletedEvent
{
    public Guid PostId { get; set; }
}