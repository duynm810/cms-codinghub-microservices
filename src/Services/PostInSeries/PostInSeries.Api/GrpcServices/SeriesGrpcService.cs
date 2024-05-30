using PostInSeries.Api.GrpcServices.Interfaces;
using Series.Grpc.Protos;
using Shared.Dtos.Series;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class SeriesGrpcService(SeriesProtoService.SeriesProtoServiceClient seriesProtoServiceClient, ILogger logger) : ISeriesGrpcService
{
    public async Task<SeriesDto?> GetSeriesById(Guid id)
    {
        try
        {
            var request = new GetSeriesByIdRequest() { Id = id.ToString() };
            var result = await seriesProtoServiceClient.GetSeriesByIdAsync(request);
            if (result != null)
            {
                return new SeriesDto()
                {
                    Id = Guid.Parse(result.Id),
                    Title = result.Title,
                    Slug = result.Slug
                };
            }

            return null;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeriesById), e);
            return null;
        }
    }
}