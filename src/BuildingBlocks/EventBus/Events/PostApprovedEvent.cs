using EventBus.Commons;
using EventBus.Events.Interfaces;

namespace EventBus.Events;

public record PostApprovedEvent : IntergrationEvent, IPostApprovedEvent
{
    public Guid PostId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }
}