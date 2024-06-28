using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents.Posts;

public record PostCreatedEvent : IntegrationEvent, IPostCreatedEvent
{
    public Guid PostId { get; set; }

    public List<RawTagDto> RawTags { get; set; } = [];
}