using Contracts.Commons.Interfaces;
using Grpc.Core;
using PostInTag.Grpc.Protos;
using Shared.Constants;
using Shared.Helpers;
using Tag.Api.GrpcClients.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.GrpcClients;

public class PostInTagGrpcClient(PostInTagService.PostInTagServiceClient postInTagServiceClient, ICacheService cacheService, ILogger logger) : IPostInTagGrpcClient
{
    public async Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId)
    {
        const string methodName = nameof(GetTagIdsByPostIdAsync);

        try
        {
            var request = new GetTagsByPostIdRequest
            {
                PostId = postId.ToString()
            };

            var result = await postInTagServiceClient.GetTagsByPostIdAsync(request);
            if (result == null || result.TagIds.Count == 0)
            {
                logger.Warning("{MethodName}: No tags found for post id {Id}", methodName, postId);
                return Enumerable.Empty<Guid>();
            }
        
            var tagIds = result.TagIds.Select(Guid.Parse);
            
            var tagIdList = tagIds as Guid[] ?? tagIds.ToArray();
            
            return tagIdList;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting tags by post id {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, postId, rpcEx.StatusCode, rpcEx.Message);
            return Enumerable.Empty<Guid>();
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting tags by post id {Id}. Message: {ErrorMessage}", methodName, postId, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}