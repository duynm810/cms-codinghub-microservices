using Post.Application.Commons.Mappings;
using Post.Domain.Entities;
using Shared.Enums;

namespace Post.Application.Commons.Models;

public class PostActivityLogDto : IMapFrom<PostActivityLog>
{
    public string? Note { get; set; }

    public PostStatusEnum FromStatus { get; set; }

    public PostStatusEnum ToStatus { get; set; }
   
    public Guid PostId { get; set; }

    public Guid UserId { get; set; }
}