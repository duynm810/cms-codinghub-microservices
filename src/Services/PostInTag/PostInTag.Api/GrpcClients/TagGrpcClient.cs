using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInTag.Api.GrpcClients.Interfaces;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Tag.Grpc.Protos;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.GrpcClients;

public class TagGrpcClient(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcClient
{
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
            
            // Log request data
            logger.Information("Sending gRPC request for GetTagBySlug with Slug: {TagSlug}", slug);
            
            var result = await tagProtoServiceClient.GetTagBySlugAsync(request);
            
            // Log response data
            logger.Information("Received gRPC response for GetTagBySlug with Tag: {TagData}", result);
            
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
}