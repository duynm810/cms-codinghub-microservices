using Grpc.Core;
using PostInSeries.Grpc.Protos;
using PostInSeries.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Grpc.Services;

public class PostInSeriesService(IPostInSeriesRepository postInSeriesRepository, ILogger logger) : Protos.PostInSeriesService.PostInSeriesServiceBase
{
    public override async Task<GetPostIdsInSeriesResponse> GetPostIdsInSeries(GetPostIdsInSeriesRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetPostIdsInSeries);
        
        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post IDs for series with ID: {SeriesId}", methodName, request.SeriesId);

            var postIds = await postInSeriesRepository.GetPostIdsBySeriesId(Guid.Parse(request.SeriesId));

            if (postIds == null || postIds.Count == 0)
            {
                logger.Warning("{MethodName} - No posts found for series with ID: {SeriesId}", methodName, request.SeriesId);
                return new GetPostIdsInSeriesResponse();
            }

            var postIdList = postIds.Select(id => id.ToString()).ToList();
            
            var response = new GetPostIdsInSeriesResponse();
            response.PostIds.AddRange(postIdList);

            logger.Information("END {MethodName} - Successfully retrieved post IDs for series with ID: {SeriesId}", methodName, request.SeriesId);

            return response;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}. RPC error occurred while getting posts for series with ID: {SeriesId}. Status: {StatusCode}, Detail: {Detail}", methodName, request.SeriesId, rpcEx.StatusCode, rpcEx.Status.Detail);
            throw;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, "An unhandled exception occurred"));
        }
    }
}