using AutoMapper;
using Series.Grpc.Entities;
using Series.Grpc.Protos;

namespace Series.Grpc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SeriesBase, SeriesModel>();
    }
}