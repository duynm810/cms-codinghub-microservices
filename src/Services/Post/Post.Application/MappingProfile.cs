using AutoMapper;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.PostActivity;

namespace Post.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigurePostActivityLogMappings();
        ConfigurePostMappings();
    }

    private void ConfigurePostActivityLogMappings()
    {
        CreateMap<PostActivityLog, PostActivityLogDto>();
    }
    
    private void ConfigurePostMappings()
    {
        CreateMap<PostBase, PostDto>();
        CreateMap<CategoryDto, PostDto>()
            .ForMember(dest => dest.Category, 
                opt => opt.MapFrom(src => src));
    }
}