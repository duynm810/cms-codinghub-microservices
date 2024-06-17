using AutoMapper;
using Identity.Grpc.Entities;
using Identity.Grpc.Protos;

namespace Identity.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}