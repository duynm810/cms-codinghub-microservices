using AutoMapper;
using Contracts.Commons.Interfaces;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Category;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Tag.Grpc.Protos;

namespace Post.Infrastructure.GrpcServices;

public class TagGrpcService(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcService
{
    public async Task<IEnumerable<TagDto>> GetTagsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();
            
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.TagGrpc.GetGrpcTagsByIdsKey(idList);
            var cachedTags = await cacheService.GetAsync<IEnumerable<TagDto>>(cacheKey);
            if (cachedTags != null)
            {
                return cachedTags;
            }

            var request = new GetTagsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await tagProtoServiceClient.GetTagsByIdsAsync(request);
            var tagsByIds = mapper.Map<IEnumerable<TagDto>>(result);

            var data = tagsByIds.ToList();
            
            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public async Task<IEnumerable<TagDto>> GetTags()
    {
        const string methodName = nameof(GetTags);

        try
        {
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.TagGrpc.GetAllTagsKey();
            var cachedTags = await cacheService.GetAsync<IEnumerable<TagDto>>(cacheKey);
            if (cachedTags != null)
            {
                return cachedTags;
            }
            
            var request = new GetTagsRequest();
            var result = await tagProtoServiceClient.GetTagsAsync(request);

            var tags = mapper.Map<IEnumerable<TagDto>>(result);

            var data = tags.ToList();
            
            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}