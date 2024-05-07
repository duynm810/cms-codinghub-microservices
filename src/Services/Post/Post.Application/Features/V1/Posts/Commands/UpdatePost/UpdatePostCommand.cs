using AutoMapper;
using MediatR;
using Post.Application.Commons.Mappings;
using Post.Application.Commons.Models;
using Post.Application.Features.V1.Posts.Commons;
using Post.Domain.Entities;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.UpdatePost;

public class UpdatePostCommand : CreateOrUpdateCommand, IMapFrom<PostBase>, IRequest<ApiResult<PostDto>>
{
    public Guid Id { get; private set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public new void Mapping(Profile profile)
    {
        profile.CreateMap<UpdatePostCommand, PostBase>();
    }
}