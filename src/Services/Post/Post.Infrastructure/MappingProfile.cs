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
        #region Category

        CreateMap<CategoryDto, CategoryModel>()
            .ForMember(dest => dest.Icon,
                opt => opt.MapFrom(src => src.Icon ?? string.Empty))
            .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color ?? string.Empty))
            .ReverseMap();

        CreateMap<RepeatedField<CategoryModel>, IEnumerable<CategoryDto>>().ConvertUsing(src => ConvertCategoryModelToDto(src));
        CreateMap<GetCategoriesByIdsResponse, IEnumerable<CategoryDto>>().ConvertUsing(src => ConvertCategoryModelToDto(src.Categories));
        CreateMap<GetAllNonStaticPageCategoriesResponse, IEnumerable<CategoryDto>>().ConvertUsing(src => ConvertCategoryModelToDto(src.Categories));

        #endregion

        #region Tag

        CreateMap<TagDto, TagModel>().ReverseMap();
        CreateMap<RepeatedField<TagModel>, IEnumerable<TagDto>>().ConvertUsing(src => ConvertTagModelToDto(src));
        CreateMap<GetTagsByIdsResponse, IEnumerable<TagDto>>().ConvertUsing(src => ConvertTagModelToDto(src.Tags));
        CreateMap<GetTagsResponse, IEnumerable<TagDto>>().ConvertUsing(src => ConvertTagModelToDto(src.Tags));

        #endregion
    }
    
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

    private IEnumerable<TagDto> ConvertTagModelToDto(IEnumerable<TagModel> tags)
    {
        return tags.Select(x => new TagDto()
        {
            Id = Guid.Parse(x.Id),
            Name = x.Name
        }).ToList();
    }
}