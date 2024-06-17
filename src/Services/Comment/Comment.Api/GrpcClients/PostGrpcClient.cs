using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using Post.Grpc.Protos;
using Shared.Constants;
using Shared.Dtos.Post;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace Comment.Api.GrpcClients;

public class PostGrpcClient(
    PostProtoService.PostProtoServiceClient postProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IPostGrpcClient
{
    public async Task<IEnumerable<PostDto>?> GetTop10Posts()
    {
        const string methodName = nameof(GetTop10Posts);

        try
        {
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.PostGrpc.GetTop10PostsKey();
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostDto>>(cacheKey);
            if (cachedPosts != null)
            {
                return cachedPosts;
            }

            var request = new GetTop10PostsRequest();

            var result = await postProtoServiceClient.GetTop10PostsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                var posts = mapper.Map<IEnumerable<PostDto>>(result.Posts);
                var data = posts.ToList();

                // Lưu cache
                await cacheService.SetAsync(cacheKey, data);

                return data;
            }

            logger.Warning("{MethodName}: No posts found", methodName);
            return Enumerable.Empty<PostDto>();
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting top 10 posts. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            return Enumerable.Empty<PostDto>();
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting top 10 posts. Message: {ErrorMessage}", methodName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}