using AutoMapper;
using Series.Api.Entities;
using Shared.Dtos.Series;
using Shared.Requests.Series;

namespace Series.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SeriesBase, SeriesDto>();
        CreateMap<CreateSeriesRequest, SeriesBase>();
        CreateMap<UpdateSeriesRequest, SeriesBase>();
    }
}