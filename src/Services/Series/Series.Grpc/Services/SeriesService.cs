using Grpc.Core;
using Series.Grpc.Protos;
using Series.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Series.Grpc.Services;

public class SeriesService(ISeriesRepository seriesRepository, ILogger logger)
    : SeriesProtoService.SeriesProtoServiceBase
{
    public override async Task<SeriesModel?> GetSeriesById(GetSeriesByIdRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetSeriesById);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting series by ID: {SeriesId}", methodName, request.Id);

            var series = await seriesRepository.GetSeriesById(Guid.Parse(request.Id));
            if (series == null)
            {
                logger.Warning("{MethodName} - Series not found for ID: {SeriesId}", methodName, request.Id);
                return null;
            }

            var data = new SeriesModel
            {
                Id = series.Id.ToString(),
                Name = series.Name,
            };

            logger.Information(
                "END {MethodName} - Success: Retrieved Category {SeriesId} - Name: {CategoryName}",
                methodName, data.Id, data.Name);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            return null;
        }
    }
}