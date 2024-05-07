using AutoMapper;
using MediatR;
using Post.Application.Commons.Mappings;
using Post.Application.Features.V1.Posts.Commons;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommand : CreateOrUpdateCommand, IMapFrom<CreateOrUpdatePostDto>, IRequest<ApiResult<Guid>>
{
    public new void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrUpdatePostDto, CreatePostCommand>();
    }
}