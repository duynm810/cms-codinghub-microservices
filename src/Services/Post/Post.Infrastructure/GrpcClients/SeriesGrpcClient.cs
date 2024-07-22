using AutoMapper;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using Post.Domain.GrpcClients;
using Series.Grpc.Protos;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Series;

namespace Post.Infrastructure.GrpcClients;

public class SeriesGrpcClient(
    SeriesProtoService.SeriesProtoServiceClient seriesProtoServiceClient,
    IMapper mapper,
    ILogger logger) : ISeriesGrpcClient
{
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
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting series by slug: {Slug}. Message: {ErrorMessage}", methodName, slug, e.Message);
            throw;
        }
    }
}