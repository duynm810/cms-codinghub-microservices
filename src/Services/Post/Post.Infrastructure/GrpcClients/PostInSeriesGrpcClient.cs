using Grpc.Core;
using Post.Domain.GrpcClients;
using PostInSeries.Grpc.Protos;
using Serilog;
using Shared.Constants;

namespace Post.Infrastructure.GrpcClients;

public class PostInSeriesGrpcClient(PostInSeriesService.PostInSeriesServiceClient postInSeriesGrpcClient, ILogger logger) : IPostInSeriesGrpcClient
{
    public async Task<IEnumerable<Guid>> GetPostIdsInSeries(Guid seriesId)
    {
        const string methodName = nameof(GetPostIdsInSeries);

        try
        {
            var request = new GetPostIdsInSeriesRequest()
            {
                SeriesId = seriesId.ToString()
            };

            var result = await postInSeriesGrpcClient.GetPostIdsInSeriesAsync(request);
            
            if (result == null || result.PostIds.Count == 0)
            {
                logger.Warning("{MethodName}: No posts found for series id {Id}", methodName, seriesId);
                return Enumerable.Empty<Guid>();
            }
            
            var postIds = result.PostIds.Select(Guid.Parse);

            var postIdList = postIds as Guid[] ?? postIds.ToArray();
            return postIdList;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx,
                "{MethodName}: gRPC error occurred while getting posts by series id {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}",
                methodName, seriesId, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}: Unexpected error occurred while getting posts by series id {Id}. Message: {ErrorMessage}",
                methodName, seriesId, e.Message);
            throw;
        }
    }
}