using Contracts.Domains;
using Infrastructure.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enums;

namespace Comment.Api.Entities;

[BsonCollection("Comments")]
public class CommentBase : MongoEntity
{
    /// <summary>
    /// ID người dùng
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public required Guid UserId { get; set; }

    /// <summary>
    /// ID bài viết
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public required Guid PostId { get; set; }

    /// <summary>
    /// Nội dung bình luận
    /// </summary>
    public required string Content { get; set; }
    
    /// <summary>
    /// ID bình luận cha
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Số lượt thích
    /// </summary>
    public int? Likes { get; set; } = 0;

    /// <summary>
    /// Số lượt phản hồi
    /// </summary>
    public int RepliesCount { get; set; } = 0;

    /// <summary>
    /// Trạng thái bình luận
    /// </summary>
    public CommentStatusEnum Status { get; set; } = CommentStatusEnum.Pending;
}