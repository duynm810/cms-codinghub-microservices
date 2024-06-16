using AutoMapper;
using Identity.Grpc.Entities;
using Identity.Grpc.Protos;

namespace Identity.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.UserId, opt =>
                opt.MapFrom(src => src.Id.ToString()));
    }
}