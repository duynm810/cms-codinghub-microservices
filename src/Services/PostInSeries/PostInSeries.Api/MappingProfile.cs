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
        ConfigurePostMappings();
        ConfigureSeriesMappings();
        ConfigurePostInSeriesMappings();
        ConfigureCategoryGrpcMappings();
    }

    private void ConfigurePostMappings()
    {
        CreateMap<PostModel, PostInSeriesDto>();
    }

    private void ConfigureSeriesMappings()
    {
        CreateMap<SeriesModel, SeriesDto>().ReverseMap();
    }

    private void ConfigurePostInSeriesMappings()
    {
        CreateMap<CreatePostInSeriesDto, PostInSeriesBase>();
        CreateMap<DeletePostInSeriesDto, PostInSeriesBase>();
    }

    private void ConfigureCategoryGrpcMappings()
    {
        CreateMap<CategoryDto, CategoryModel>()
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon ?? string.Empty))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color ?? string.Empty))
            .ReverseMap();

        CreateMap<RepeatedField<CategoryModel>, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => src.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SeoDescription = c.SeoDescription,
                Icon = c.Icon,
                Color = c.Color
            }).ToList());

        CreateMap<GetCategoriesByIdsResponse, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => src.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SeoDescription = c.SeoDescription,
                Icon = c.Icon,
                Color = c.Color
            }).ToList());

        CreateMap<CategoryDto, PostInSeriesDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Slug));
    }
}