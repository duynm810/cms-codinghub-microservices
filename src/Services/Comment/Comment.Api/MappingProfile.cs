using AutoMapper;
using Comment.Api.Entities;
using Post.Grpc.Protos;
using Shared.Dtos.Comment;
using Shared.Dtos.Post;

namespace Comment.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureCommentMappings();
        ConfigurePostGrpcMappings();
    }

    private void ConfigureCommentMappings()
    {
        CreateMap<CommentBase, CommentDto>().ReverseMap();
        CreateMap<CommentBase, CreateCommentDto>().ReverseMap();
    }

    private void ConfigurePostGrpcMappings()
    {
        CreateMap<PostModel, PostDto>().ReverseMap();
        CreateMap<PostDto, GetTop10PostsResponse>().ReverseMap();
        CreateMap<GetTop10PostsResponse, IEnumerable<PostDto>>()
            .ConvertUsing(src => src.Posts.Select(p => new PostDto
            {
                Id = Guid.Parse(p.Id),
                Title = p.Title,
                Slug = p.Slug
            }).ToList());
    }
}