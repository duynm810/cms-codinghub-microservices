using AutoMapper;
using Contracts.Commons.Interfaces;
using Post.Domain.GrpcClients;
using Serilog;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Tag.Grpc.Protos;

namespace Post.Infrastructure.GrpcClients;

public class TagGrpcClient(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcClient
{
    public async Task<IEnumerable<TagDto>> GetTagsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();
            
            // Check existed cache (Kiểm tra cache)
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
            
            // Save cache (Lưu cache)
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
            // Check existed cache (Kiểm tra cache)
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
            
            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
    
    public async Task<TagDto?> GetTagBySlug(string slug)
    {
        const string methodName = nameof(GetTagBySlug);
        
        try
        {
            var cacheKey = CacheKeyHelper.TagGrpc.GetGrpcTagBySlugKey(slug);

            // Check existed cache (Kiểm tra cache)
            var cachedTag = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cachedTag != null)
            {
                return cachedTag;
            }

            var request = new GetTagBySlugRequest() { Slug = slug };
            var result = await tagProtoServiceClient.GetTagBySlugAsync(request);
            var data = mapper.Map<TagDto>(result);

            // Save cache (Lưu cache)
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