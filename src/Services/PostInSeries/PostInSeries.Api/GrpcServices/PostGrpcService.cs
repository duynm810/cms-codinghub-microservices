using AutoMapper;
using Contracts.Commons.Interfaces;
using Post.Grpc.Protos;
using PostInSeries.Api.GrpcServices.Interfaces;
using Shared.Dtos.PostInSeries;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class PostGrpcService(PostProtoService.PostProtoServiceClient postProtoServiceClient, ICacheService cacheService, IMapper mapper, ILogger logger)
    : IPostGrpcService
{
    public async Task<IEnumerable<PostInSeriesDto>> GetPostsByIds(IEnumerable<Guid> ids)
    {
        var idList = ids as Guid[] ?? ids.ToArray();

        try
        {
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.PostGrpc.GetGrpcPostsByIdsKey(idList);
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostInSeriesDto>>(cacheKey);
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
                var postsByIds =  mapper.Map<IEnumerable<PostInSeriesDto>>(result.Posts);
                var data = postsByIds.ToList();
                
                // Lưu cache
                await cacheService.SetAsync(cacheKey, data);
                
                return data;
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsByIds), e);
            throw;
        }

        return new List<PostInSeriesDto>();
    }
}