using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Post.Grpc.Protos;
using PostInTag.Api.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.PostInTag;
using Shared.Dtos.Tag;
using Tag.Grpc.Protos;

namespace PostInTag.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Post

        CreateMap<PostModel, PostInTagDto>();

        #endregion

        #region Tag

        CreateMap<TagModel, TagDto>().ReverseMap();

        #endregion

        #region Post In Tag

        CreateMap<CreatePostInTagDto, PostInTagBase>();
        CreateMap<DeletePostInTagDto, PostInTagBase>();

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
        CreateMap<CategoryDto, PostInSeriesDto>()
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

    #endregion
}