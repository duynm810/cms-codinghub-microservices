using AutoMapper;
using Category.Grpc.Entities;
using Category.Grpc.Protos;

namespace Category.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryBase, CategoryModel>()
            .ForMember(dest => dest.Icon,
                opt => opt.MapFrom(src => src.Icon ?? string.Empty))
            .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color ?? string.Empty));

        CreateMap<IEnumerable<CategoryBase>, GetCategoriesByIdsResponse>()
            .ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src));

        CreateMap<IEnumerable<CategoryBase>, GetAllNonStaticPageCategoriesResponse>()
            .ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src));
    }
}