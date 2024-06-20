using AutoMapper;
using MediatR;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Features.V1.Posts.Commons;
using Post.Domain.Entities;
using Shared.Dtos.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommand : CreateOrUpdateCommand, IMapFrom<CreatePostDto>, IRequest<ApiResult<Guid>>
{
    public Guid AuthorUserId { get; set; }

    public new void Mapping(Profile profile)
    {
        profile.CreateMap<CreatePostDto, CreatePostCommand>();
        profile.CreateMap<CreatePostCommand, PostBase>()
            .ForMember(dest => dest.AuthorUserId,
                opt => opt.MapFrom(src => src.AuthorUserId));
    }
}