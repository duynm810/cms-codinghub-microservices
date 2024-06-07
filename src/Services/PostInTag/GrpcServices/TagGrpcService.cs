using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInTag.Api.GrpcServices.Interfaces;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Tag.Grpc.Protos;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.GrpcServices;

public class TagGrpcService(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcService
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