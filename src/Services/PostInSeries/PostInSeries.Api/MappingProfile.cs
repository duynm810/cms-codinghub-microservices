using AutoMapper;
using Post.Grpc.Protos;
using Series.Grpc.Protos;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.Series;

namespace PostInSeries.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Post

        CreateMap<PostModel, PostInSeriesDto>();

        #endregion

        #region Series

        CreateMap<SeriesModel, SeriesDto>().ReverseMap();

        #endregion
    }
}