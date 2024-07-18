using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Series.Grpc.Protos;
using Series.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Series.Grpc.Services;

public class SeriesService(ISeriesRepository seriesRepository, IMapper mapper, ILogger logger)
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

            var data = mapper.Map<SeriesModel>(series);

            logger.Information(
                "END {MethodName} - Success: Retrieved Series {SeriesId} - Title: {SeriesName}",
                methodName, data.Id, data.Title);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public override async Task<SeriesModel?> GetSeriesBySlug(GetSeriesBySlugRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetSeriesBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting series by Slug: {SeriesSlug}", methodName, request.Slug);

            var series = await seriesRepository.GetSeriesBySlug(request.Slug);
            if (series == null)
            {
                logger.Warning("{MethodName} - Series not found for Slug: {SeriesSlug}", methodName, request.Slug);
                return null;
            }

            var data = mapper.Map<SeriesModel>(series);

            logger.Information(
                "END {MethodName} - Success: Retrieved Series {SeriesId} - Title: {SeriesName}",
                methodName, data.Id, data.Title);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public override async Task<GetAllSeriesResponse> GetAllSeries(Empty request, ServerCallContext context)
    {
        const string methodName = nameof(GetAllSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting all series", methodName);
        
            var seriesList = await seriesRepository.GetAllSeries();
            
            var data = new GetAllSeriesResponse();
            data.Series.AddRange(seriesList.Select(mapper.Map<SeriesModel>));

            logger.Information("END {MethodName} - Success: Retrieved all series", methodName);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public override async Task<GetSeriesByIdsResponse> GetSeriesByIds(GetSeriesByIdsRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetSeriesByIds);

        try
        {
            var seriesIds = request.Ids.Select(Guid.Parse).ToArray();

            logger.Information("{MethodName} - Beginning to retrieve series for IDs: {SeriesIds}", methodName,
                seriesIds);

            var series = await seriesRepository.GetSeriesByIds(seriesIds);

            var data = mapper.Map<GetSeriesByIdsResponse>(series);

            logger.Information("{MethodName} - Successfully retrieved {Count} series.", methodName,
                data.Series.Count);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting series by IDs: {SeriesIds}. Message: {ErrorMessage}",
                methodName, request.Ids, e.Message);
            throw new RpcException(new Status(StatusCode.Internal,
                "An error occurred while getting series by IDs"));
        }
    }
}