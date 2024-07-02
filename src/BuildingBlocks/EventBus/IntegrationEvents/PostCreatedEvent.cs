using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents;

public record PostCreatedEvent(string SourceService) : IntegrationEvent(SourceService), IPostCreatedEvent
{
    public Guid PostId { get; set; }

    public List<RawTagDto> RawTags { get; set; } = [];
}