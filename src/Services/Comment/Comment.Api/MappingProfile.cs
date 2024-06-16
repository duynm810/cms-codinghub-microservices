using AutoMapper;
using Comment.Api.Entities;
using Identity.Grpc.Protos;
using Post.Grpc.Protos;
using Shared.Dtos.Comment;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post;

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

    private void ConfigureIdentityGrpcMappings()
    {
        CreateMap<UserResponse, UserDto>()
            .ForMember(dest => dest.UserId, opt =>
                opt.MapFrom(src => Guid.Parse(src.UserId)));

        CreateMap<UserRequest, UserDto>().ReverseMap();

        CreateMap<UsersResponse, List<UserDto>>()
            .ConvertUsing(src => src.Users.Select(u => new UserDto
            {
                UserId = Guid.Parse(u.UserId),
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToList());
    }
}