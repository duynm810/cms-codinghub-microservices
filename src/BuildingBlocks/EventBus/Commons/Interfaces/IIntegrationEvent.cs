namespace EventBus.Commons.Interfaces;

public interface IIntegrationEvent
{
    /// <summary>
    /// Id của sự kiện, là giá trị duy nhất
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Thời gian tạo sự kiện
    /// </summary>
    DateTimeOffset CreationDate { get; set; }
    
    /// <summary>
    /// Loại sự kiện.
    /// </summary>
    string EventType { get; set; }
    
    /// <summary>
    /// Id liên kết để theo dõi chuỗi sự kiện liên quan.
    /// </summary>
    Guid? CorrelationId { get; set; }
    
    /// <summary>
    /// Tên của service đã tạo ra sự kiện.
    /// </summary>
    string SourceService { get; set; }
}