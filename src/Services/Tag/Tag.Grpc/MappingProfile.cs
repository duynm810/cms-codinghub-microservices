using AutoMapper;
using Tag.Grpc.Entities;
using Tag.Grpc.Protos;

namespace Tag.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TagBase, TagModel>();
        
        CreateMap<IEnumerable<TagBase>, GetTagsByIdsResponse>()
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src));
        
        CreateMap<IEnumerable<TagBase>, GetTagsResponse>()
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src));
    }
}