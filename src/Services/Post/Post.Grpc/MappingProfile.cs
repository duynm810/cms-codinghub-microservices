using AutoMapper;
using Post.Domain.Entities;
using Post.Grpc.Protos;

namespace Post.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PostBase, PostModel>();

        CreateMap<IEnumerable<PostBase>, GetPostsByIdsResponse>().ForMember(dest => dest.Posts,
            opt => opt.MapFrom(src => src));
    }
}