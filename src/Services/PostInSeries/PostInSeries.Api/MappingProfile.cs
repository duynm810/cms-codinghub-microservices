using AutoMapper;
using Post.Grpc.Protos;
using Shared.Dtos.PostInSeries;

namespace PostInSeries.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PostModel, PostInSeriesDto>();
    }
}