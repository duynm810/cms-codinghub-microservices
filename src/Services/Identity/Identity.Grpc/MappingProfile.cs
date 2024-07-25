using AutoMapper;
using Identity.Grpc.Entities;
using Identity.Grpc.Protos;

namespace Identity.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.AvatarUrl ?? string.Empty))
            .ForMember(dest => dest.About,
                opt => opt.MapFrom(src => src.About ?? string.Empty));
    }
}