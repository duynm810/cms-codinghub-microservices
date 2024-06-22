using AutoMapper;
using Post.Application.Features.V1.Posts.Commands.CreatePost;
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
using Post.Application.Features.V1.Posts.Commons;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Post.Commands;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.PostActivity;
using Shared.Extensions;

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
        #region Get

        CreateMap<PostBase, PostDto>();

        #endregion

        #region Create

        CreateMap<CreatePostDto, CreatePostCommand>();
        CreateMap<CreatePostCommand, PostBase>()
            .ForMember(dest => dest.AuthorUserId,
                opt => opt.MapFrom(src => src.AuthorUserId));
        CreateMap<CreateOrUpdateCommand, PostBase>();

        #endregion

        #region Update

        CreateMap<UpdatePostDto, UpdatePostCommand>().IgnoreAllNonExisting();
        CreateMap<UpdatePostCommand, PostBase>();

        #endregion
    }
}