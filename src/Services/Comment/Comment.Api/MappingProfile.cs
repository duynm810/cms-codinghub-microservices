using AutoMapper;
using Post.Grpc.Protos;
using Shared.Dtos.Post;

namespace Comment.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigurePostGrpcMappings();
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