using AutoMapper;
using Post.Application.Features.V1.Posts.Commands.CreatePost;
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
using Post.Application.Features.V1.Posts.Commands.UpdateThumbnail;
using Post.Application.Features.V1.Posts.Commons;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Dtos.PostActivity;
using Shared.Extensions;
using Shared.Requests.Post.Commands;

namespace Post.Application.Commons.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigurePostActivityLogMappings();
        ConfigureCategoryGrpcMappings();
        ConfigurePostMappings();
    }

    private void ConfigurePostActivityLogMappings()
    {
        CreateMap<PostActivityLog, PostActivityLogDto>();
    }

    private void ConfigureCategoryGrpcMappings()
    {
        CreateMap<CategoryDto, CategoryDto>();
        CreateMap<CategoryDto, PostDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src))
            .ReverseMap();
    }

    private void ConfigurePostMappings()
    {
        #region GET

        CreateMap<PostBase, PostDto>();

        #endregion

        #region CREATE

        CreateMap<CreatePostRequest, CreatePostCommand>();
        CreateMap<CreatePostCommand, PostBase>()
            .ForMember(dest => dest.AuthorUserId,
                opt => opt.MapFrom(src => src.AuthorUserId));
        CreateMap<CreateOrUpdateCommand, PostBase>();

        #endregion

        #region UPDATE

        CreateMap<UpdatePostRequest, UpdatePostCommand>().IgnoreAllNonExisting();
        CreateMap<UpdateThumbnailRequest, UpdateThumbnailCommand>().IgnoreAllNonExisting();
        CreateMap<UpdatePostCommand, PostBase>();

        #endregion
    }
}