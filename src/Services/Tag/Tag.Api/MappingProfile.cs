using AutoMapper;
using Shared.Dtos.Tag;
using Tag.Api.Entities;

namespace Tag.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TagBase, TagDto>();
        CreateMap<CreateTagDto, TagBase>();
        CreateMap<UpdateTagDto, TagBase>();
    }
}