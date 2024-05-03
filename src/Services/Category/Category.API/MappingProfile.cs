using AutoMapper;
using Category.API.Entities;
using Shared.Dtos.Category;

namespace Category.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryBase, CategoryDto>();
    }
}