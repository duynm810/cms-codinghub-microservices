using MediatR;
using Post.Application.Features.V1.Posts.Commons;
using Shared.Constants;
using Shared.Enums;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommand : CreateOrUpdateCommand, IRequest<ApiResult<Guid>>
{
    public Guid AuthorUserId { get; set; }
    
    public PostStatusEnum Status { get; set; }
    
    public DateTimeOffset? PublishedDate { get; set; }

    public void SetStatusBasedOnRoles(IEnumerable<string> roles)
    {
        if (roles.Contains(UserRolesConsts.Administrator))
        {
            Status = PostStatusEnum.Published;
            PublishedDate = DateTimeOffset.UtcNow;
        }
    }
}