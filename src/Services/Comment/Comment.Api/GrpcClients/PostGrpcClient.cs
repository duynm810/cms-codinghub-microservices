using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Contracts.Commons.Interfaces;
using Post.Grpc.Protos;
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
    public async Task<IEnumerable<PostDto>> GetTop10Posts()
    {
        const string methodName = nameof(GetTop10Posts);

        try
        {
            // Check existed cache (Kiểm tra cache)
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

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                return data;
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }

        return new List<PostDto>();
    }
}