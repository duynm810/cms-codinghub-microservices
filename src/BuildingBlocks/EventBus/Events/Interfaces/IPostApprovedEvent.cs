using EventBus.Commons;

namespace EventBus.Events.Interfaces;

public interface IPostApprovedEvent : IIntergrationEvent
{
    Guid PostId { get; set; }
    
    string? Name { get; set; }
    
    string? Description { get; set; }
    
    string? Content { get; set; }
}