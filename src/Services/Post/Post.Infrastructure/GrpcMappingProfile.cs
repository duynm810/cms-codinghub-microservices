using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Shared.Dtos.Category;

namespace Post.Infrastructure;

public class GrpcMappingProfile : Profile
{
    public GrpcMappingProfile()
    {
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
}