using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Post.Grpc.Protos;
using PostInSeries.Api.Entities;
using Series.Grpc.Protos;
using Shared.Dtos.Category;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.Series;

namespace PostInSeries.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Post

        CreateMap<PostModel, PostInSeriesDto>();

        #endregion

        #region Series

        CreateMap<SeriesModel, SeriesDto>().ReverseMap();

        #endregion

        #region Post-In-Series

        CreateMap<CreatePostInSeriesDto, PostInSeriesBase>();
        CreateMap<DeletePostInSeriesDto, PostInSeriesBase>();

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