using AutoMapper;
using Series.Api.Entities;
using Shared.Dtos.Series;

namespace Series.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SeriesBase, SeriesDto>();
        CreateMap<CreateSeriesDto, SeriesBase>();
        CreateMap<UpdateSeriesDto, SeriesBase>();
    }
}