using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInSeries.Api.GrpcServices.Interfaces;
using Series.Grpc.Protos;
using Shared.Dtos.Series;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class SeriesGrpcService(
    SeriesProtoService.SeriesProtoServiceClient seriesProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ISeriesGrpcService
{
    public async Task<SeriesDto?> GetSeriesById(Guid id)
    {
        const string methodName = nameof(GetSeriesById);
        
        try
        {
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.SeriesGrpc.GetGrpcSeriesByIdKey(id);
            var cachedSeries = await cacheService.GetAsync<SeriesDto>(cacheKey);
            if (cachedSeries != null)
            {
                return cachedSeries;
            }

            var request = new GetSeriesByIdRequest() { Id = id.ToString() };
            var result = await seriesProtoServiceClient.GetSeriesByIdAsync(request);
            var data = mapper.Map<SeriesDto>(result);

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public async Task<SeriesDto?> GetSeriesBySlug(string slug)
    {
        const string methodName = nameof(GetSeriesBySlug);
        
        try
        {
            var cacheKey = CacheKeyHelper.SeriesGrpc.GetGrpcSeriesBySlugKey(slug);

            // Kiểm tra cache
            var cachedSeries = await cacheService.GetAsync<SeriesDto>(cacheKey);
            if (cachedSeries != null)
            {
                return cachedSeries;
            }

            var request = new GetSeriesBySlugRequest() { Slug = slug };
            var result = await seriesProtoServiceClient.GetSeriesBySlugAsync(request);
            var data = mapper.Map<SeriesDto>(result);

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}