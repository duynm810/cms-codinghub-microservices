using AutoMapper;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using PostInSeries.Api.GrpcClients.Interfaces;
using Series.Grpc.Protos;
using Shared.Constants;
using Shared.Dtos.Series;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcClients;

public class SeriesGrpcClient(
    SeriesProtoService.SeriesProtoServiceClient seriesProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ISeriesGrpcClient
{
    public async Task<SeriesDto?> GetSeriesById(Guid id)
    {
        const string methodName = nameof(GetSeriesById);

        try
        {
            var request = new GetSeriesByIdRequest { Id = id.ToString() };
            var result = await seriesProtoServiceClient.GetSeriesByIdAsync(request);
        
            if (result == null)
            {
                logger.Warning("{MethodName}: No series found with ID: {Id}", methodName, id);
                return null;
            }

            var data = mapper.Map<SeriesDto>(result);
            
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting series by ID: {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, id, rpcEx.StatusCode, rpcEx.Message);
            return null;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting series by ID: {Id}. Message: {ErrorMessage}", methodName, id, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
    
    public async Task<SeriesDto?> GetSeriesBySlug(string slug)
    {
        const string methodName = nameof(GetSeriesBySlug);

        try
        {
            var request = new GetSeriesBySlugRequest { Slug = slug };
            var result = await seriesProtoServiceClient.GetSeriesBySlugAsync(request);

            if (result == null)
            {
                logger.Warning("{MethodName}: No series found with slug: {Slug}", methodName, slug);
                return null;
            }

            var data = mapper.Map<SeriesDto>(result);
            
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting series by slug: {Slug}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, slug, rpcEx.StatusCode, rpcEx.Message);
            return null;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting series by slug: {Slug}. Message: {ErrorMessage}", methodName, slug, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }

}