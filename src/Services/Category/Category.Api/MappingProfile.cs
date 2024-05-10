using AutoMapper;
using Category.Api.Entities;
using Shared.Dtos.Category;

namespace Category.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryBase, CategoryDto>();
        CreateMap<CreateCategoryDto, CategoryBase>();
        CreateMap<UpdateCategoryDto, CategoryBase>();
    }
}