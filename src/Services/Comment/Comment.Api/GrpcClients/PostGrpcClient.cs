using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Grpc.Core;
using Post.Grpc.Protos;
using Shared.Constants;
using Shared.Dtos.Post;
using ILogger = Serilog.ILogger;

namespace Comment.Api.GrpcClients;

public class PostGrpcClient(
    PostProtoService.PostProtoServiceClient postProtoServiceClient,
    IMapper mapper,
    ILogger logger)
    : IPostGrpcClient
{
    public async Task<List<PostDto>> GetTop10Posts()
    {
        const string methodName = nameof(GetTop10Posts);

        try
        {
            var request = new GetTop10PostsRequest();

            var result = await postProtoServiceClient.GetTop10PostsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                var data = mapper.Map<List<PostDto>>(result.Posts);
                return data;
            }

            logger.Warning("{MethodName}: No posts found", methodName);
            return [];
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting top 10 posts. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting top 10 posts. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }

    public async Task<List<PostDto>> GetPostsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetPostsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();

            // Convert each GUID to its string representation
            var request = new GetPostsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await postProtoServiceClient.GetPostsByIdsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                var data = mapper.Map<List<PostDto>>(result.Posts);
                return data;
            }

            logger.Warning("{MethodName}: No posts found for the given ids", methodName);
            return [];
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting posts by ids. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting posts by ids. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }
}