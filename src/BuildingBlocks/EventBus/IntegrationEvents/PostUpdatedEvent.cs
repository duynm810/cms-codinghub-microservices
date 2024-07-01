using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents;

public record PostUpdatedEvent(string SourceService) : IntegrationEvent(SourceService), IPostUpdatedEvent
{
    public Guid PostId { get; set; }

    public List<RawTagDto> RawTags { get; set; } = [];
}