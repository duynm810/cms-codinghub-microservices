namespace EventBus.Commons;

public interface IIntergrationEvent
{
    Guid Id { get; set; }
    
    DateTimeOffset CreationDate { get; set; }
}