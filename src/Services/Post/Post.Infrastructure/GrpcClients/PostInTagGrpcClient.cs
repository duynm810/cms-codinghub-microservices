using Grpc.Core;
using Post.Domain.GrpcClients;
using PostInTag.Grpc.Protos;
using Serilog;
using Shared.Constants;

namespace Post.Infrastructure.GrpcClients;

public class PostInTagGrpcClient(PostInTagService.PostInTagServiceClient postInTagServiceClient, ILogger logger)
    : IPostInTagGrpcClient
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
            logger.Error(rpcEx,
                "{MethodName}: gRPC error occurred while getting tags by post id {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}",
                methodName, postId, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}: Unexpected error occurred while getting tags by post id {Id}. Message: {ErrorMessage}",
                methodName, postId, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Guid>> GetPostIdsInTagAsync(Guid tagId)
    {
        const string methodName = nameof(GetPostIdsInTagAsync);

        try
        {
            var request = new GetPostIdsInTagRequest
            {
                TagId = tagId.ToString()
            };

            var result = await postInTagServiceClient.GetPostIdsInTagAsync(request);
            if (result == null || result.PostIds.Count == 0)
            {
                logger.Warning("{MethodName}: No posts found for tag id {Id}", methodName, tagId);
                return Enumerable.Empty<Guid>();
            }

            var postIds = result.PostIds.Select(Guid.Parse);
            return postIds;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx,
                "{MethodName}: gRPC error occurred while getting posts by tag id {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}",
                methodName, tagId, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}: Unexpected error occurred while getting posts by tag id {Id}. Message: {ErrorMessage}",
                methodName, tagId, e.Message);
            throw;
        }
    }
}