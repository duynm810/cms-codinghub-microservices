using AutoMapper;
using Category.Api.Entities;
using Shared.Dtos.Category;
using Shared.Requests.Category;

namespace Category.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryBase, CategoryDto>();
        CreateMap<CreateCategoryRequest, CategoryBase>();
        CreateMap<UpdateCategoryRequest, CategoryBase>();
    }
}