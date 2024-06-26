using Shared.Enums;

namespace Shared.Dtos.PostActivity;

public class PostActivityLogDto
{
    /// <summary>
    /// Khoá chính
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Ghi chú liên quan đến hoạt động bài viết
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái bài viết trước khi thay đổi
    /// </summary>
    public PostStatusEnum FromStatus { get; set; }

    /// <summary>
    /// Trạng thái bài viết sau khi thay đổi
    /// </summary>
    public PostStatusEnum ToStatus { get; set; }

    /// <summary>
    ///  Khóa ngoại thuộc về bài viết nào
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Khóa ngoại người thực hiện hoạt động
    /// </summary>
    public Guid UserId { get; set; }
}