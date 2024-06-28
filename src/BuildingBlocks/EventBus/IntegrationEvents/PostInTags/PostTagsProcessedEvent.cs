using EventBus.Commons;
using EventBus.IntegrationEvents.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents.PostInTags;

public record PostTagsProcessedEvent : IntegrationEvent, IPostTagsProcessedEvent
{
    public Guid PostId { get; set; }
    
    public List<TagDto> Tags { get; set; } = [];
}