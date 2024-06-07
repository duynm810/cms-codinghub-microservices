using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Post.Grpc.Protos;
using PostInTag.Api.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.PostInTag;
using Shared.Dtos.Tag;
using Tag.Grpc.Protos;

namespace PostInTag.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Post In Tag

        CreateMap<CreatePostInTagDto, PostInTagBase>();
        CreateMap<DeletePostInTagDto, PostInTagBase>();

        #endregion

        #region Post Grpc

        CreateMap<PostModel, PostInTagDto>().ReverseMap();
        CreateMap<PostModel, PostDto>().ReverseMap();
        CreateMap<PostDto, GetTop10PostsResponse>().ReverseMap();
        CreateMap<GetTop10PostsResponse, IEnumerable<PostDto>>().ConvertUsing(src => ConvertPostModelToDto(src.Posts));

        #endregion

        #region Tag Grpc

        CreateMap<TagModel, TagDto>().ReverseMap();
        CreateMap<TagDto, GetTagsResponse>().ReverseMap();
        CreateMap<GetTagsResponse, IEnumerable<TagDto>>().ConvertUsing(src => ConvertTagModelToDto(src.Tags));

        #endregion

        #region Category Grpc

        CreateMap<CategoryDto, CategoryModel>()
            .ForMember(dest => dest.Icon,
                opt => opt.MapFrom(src => src.Icon ?? string.Empty))
            .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color ?? string.Empty))
            .ReverseMap();

        CreateMap<RepeatedField<CategoryModel>, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => ConvertCategoryModelToDto(src));

        CreateMap<GetCategoriesByIdsResponse, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => ConvertCategoryModelToDto(src.Categories));

        // Bỏ qua ánh xạ Id, Slug vì sẽ nhầm lẫn trùng field với các bảng với nhau
        CreateMap<CategoryDto, PostInTagDto>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.Slug,
                opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategorySlug,
                opt => opt.MapFrom(src => src.Slug));

        #endregion
    }

    #region HELPERS

    private IEnumerable<CategoryDto> ConvertCategoryModelToDto(IEnumerable<CategoryModel> categories)
    {
        return categories.Select(x => new CategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            Slug = x.Slug,
            SeoDescription = x.SeoDescription,
            Icon = x.Icon,
            Color = x.Color,
        }).ToList();
    }

    private IEnumerable<PostDto> ConvertPostModelToDto(IEnumerable<PostModel> posts)
    {
        return posts.Select(x => new PostDto()
        {
            Id = Guid.Parse(x.Id),
            Title = x.Title,
            Slug = x.Slug
        }).ToList();
    }

    private IEnumerable<TagDto> ConvertTagModelToDto(IEnumerable<TagModel> tags)
    {
        return tags.Select(x => new TagDto()
        {
            Id = Guid.Parse(x.Id),
            Name = x.Name,
            Slug = x.Slug
        }).ToList();
    }

    #endregion
}