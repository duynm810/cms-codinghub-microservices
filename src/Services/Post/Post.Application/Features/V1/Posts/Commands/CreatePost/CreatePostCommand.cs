using MediatR;
using Post.Application.Features.V1.Posts.Commons;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommand : CreateOrUpdateCommand, IRequest<ApiResult<Guid>>
{
    
}