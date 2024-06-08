using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Shared.Dtos.Category;
using Shared.Dtos.Tag;
using Tag.Grpc.Protos;

namespace Post.Infrastructure;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureCategoryMappings();
        ConfigureTagMappings();
    }

    private void ConfigureCategoryMappings()
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

        CreateMap<GetAllNonStaticPageCategoriesResponse, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => src.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SeoDescription = c.SeoDescription,
                Icon = c.Icon,
                Color = c.Color
            }).ToList());
    }

    private void ConfigureTagMappings()
    {
        CreateMap<TagDto, TagModel>().ReverseMap();

        CreateMap<TagDto, GetTagsResponse>().ReverseMap();

        CreateMap<RepeatedField<TagModel>, IEnumerable<TagDto>>()
            .ConvertUsing(src => src.Select(t => new TagDto
            {
                Id = Guid.Parse(t.Id),
                Name = t.Name,
                Slug = t.Slug
            }).ToList());

        CreateMap<GetTagsByIdsResponse, IEnumerable<TagDto>>()
            .ConvertUsing(src => src.Tags.Select(t => new TagDto
            {
                Id = Guid.Parse(t.Id),
                Name = t.Name,
                Slug = t.Slug
            }).ToList());

        CreateMap<GetTagsResponse, IEnumerable<TagDto>>()
            .ConvertUsing(src => src.Tags.Select(t => new TagDto
            {
                Id = Guid.Parse(t.Id),
                Name = t.Name,
                Slug = t.Slug
            }).ToList());
    }
}