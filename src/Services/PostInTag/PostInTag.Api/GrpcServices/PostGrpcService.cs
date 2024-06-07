using AutoMapper;
using Contracts.Commons.Interfaces;
using Post.Grpc.Protos;
using PostInTag.Api.GrpcServices.Interfaces;
using Shared.Dtos.PostInTag;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.GrpcServices;

public class PostGrpcService(
    PostProtoService.PostProtoServiceClient postProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IPostGrpcService
{
    public async Task<IEnumerable<PostInTagDto>> GetPostsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetPostsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.PostGrpc.GetGrpcPostsByIdsKey(idList);
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostInTagDto>>(cacheKey);
            if (cachedPosts != null)
            {
                return cachedPosts;
            }

            // Convert each GUID to its string representation
            var request = new GetPostsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await postProtoServiceClient.GetPostsByIdsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                var postsByIds = mapper.Map<IEnumerable<PostInTagDto>>(result.Posts);
                var data = postsByIds.ToList();

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

        return new List<PostInTagDto>();
    }
}