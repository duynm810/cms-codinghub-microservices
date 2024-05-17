namespace EventBus.Commons.Interfaces;

public interface IIntergrationEvent
{
    Guid Id { get; set; }
    
    DateTimeOffset CreationDate { get; set; }
}