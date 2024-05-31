using AutoMapper;
using PostInSeries.Api.GrpcServices.Interfaces;
using Series.Grpc.Protos;
using Shared.Dtos.Series;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class SeriesGrpcService(SeriesProtoService.SeriesProtoServiceClient seriesProtoServiceClient, IMapper mapper, ILogger logger) : ISeriesGrpcService
{
    public async Task<SeriesDto?> GetSeriesById(Guid id)
    {
        try
        {
            var request = new GetSeriesByIdRequest() { Id = id.ToString() };
            var result = await seriesProtoServiceClient.GetSeriesByIdAsync(request);
            var data = mapper.Map<SeriesDto>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeriesById), e);
            throw;
        }
    }

    public async Task<SeriesDto?> GetSeriesBySlug(string slug)
    {
        try
        {
            var request = new GetSeriesBySlugRequest() { Slug = slug };
            var result = await seriesProtoServiceClient.GetSeriesBySlugAsync(request);
            var data = mapper.Map<SeriesDto>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeriesById), e);
            throw;
        }
    }
}