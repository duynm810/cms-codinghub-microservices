using AutoMapper;
using Comment.Api.Entities;
using Identity.Grpc.Protos;
using Post.Grpc.Protos;
using Shared.Dtos.Comment;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;

namespace Comment.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureCommentMappings();
        ConfigurePostGrpcMappings();
        ConfigureIdentityGrpcMappings();
    }

    private void ConfigureCommentMappings()
    {
        CreateMap<CommentBase, CommentDto>().ReverseMap();
        CreateMap<CommentBase, LatestCommentDto>().ReverseMap();
        CreateMap<CommentBase, CreateCommentDto>().ReverseMap();
    }

    private void ConfigurePostGrpcMappings()
    {
        CreateMap<PostModel, PostDto>().ReverseMap();
        CreateMap<PostDto, GetTop10PostsResponse>().ReverseMap();
        CreateMap<GetTop10PostsResponse, List<PostDto>>()
            .ConvertUsing(src => src.Posts.Select(p => new PostDto
            {
                Id = Guid.Parse(p.Id),
                Title = p.Title,
                Slug = p.Slug
            }).ToList());
        
        CreateMap<List<PostDto>, GetPostsByIdsResponse>().ForMember(dest => dest.Posts,
            opt => opt.MapFrom(src => src));
    }

    private void ConfigureIdentityGrpcMappings()
    {
        CreateMap<UserRequest, UserDto>().ReverseMap();

        CreateMap<UserResponse, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? Guid.Empty : Guid.Parse(src.Id)))
            .ReverseMap();

        CreateMap<UsersResponse, List<UserDto>>()
            .ConvertUsing(src => src.Users.Select(u => new UserDto
            {
                Id = Guid.Parse(u.Id),
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName
            }).ToList());
    }
}