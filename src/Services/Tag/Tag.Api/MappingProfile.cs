using AutoMapper;
using Shared.Dtos.Tag;
using Shared.Requests.Tag;
using Tag.Api.Entities;

namespace Tag.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TagBase, TagDto>();
        CreateMap<CreateTagRequest, TagBase>();
        CreateMap<UpdateTagRequest, TagBase>();
    }
}