using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Distributed;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Category;

namespace Post.Infrastructure.GrpcServices;

public class CategoryGrpcService(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    IMapper mapper,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    ILogger logger) : ICategoryGrpcService
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        try
        {
            var cacheKey = $"category_{id}";

            // Kiểm tra cache
            var cachedCategory = await redisCacheService.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCategory))
            {
                var cachedData = serializeService.Deserialize<CategoryDto>(cachedCategory);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }
            
            var request = new GetCategoryByIdRequest { Id = id };
            var result = await categoryProtoServiceClient.GetCategoryByIdAsync(request);
            var data = mapper.Map<CategoryDto>(result);
            
            // Lưu cache
            var serializedData = serializeService.Serialize(data);
            await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
            });
            
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        var idList = ids as long[] ?? ids.ToArray();
        
        try
        {
            var cacheKey = $"categories_{string.Join("_", idList)}";

            // Kiểm tra cache
            var cachedCategories = await redisCacheService.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCategories))
            {
                var cachedData = serializeService.Deserialize<IEnumerable<CategoryDto>>(cachedCategories);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }
            
            var request = new GetCategoriesByIdsRequest() { Ids = { idList } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            var data = mapper.Map<IEnumerable<CategoryDto>>(result);
            
            // Lưu cache
            var categoriesByIds = data.ToList();
            
            var serializedData = serializeService.Serialize(categoriesByIds);
            await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
            });

            return categoriesByIds;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoriesByIds), e);
            throw;
        }
    }

    public async Task<CategoryDto?> GetCategoryBySlug(string slug)
    {
        try
        { 
            var cacheKey = $"category_slug_{slug}";

            // Kiểm tra cache
            var cachedCategory = await redisCacheService.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCategory))
            {
                var cachedData = serializeService.Deserialize<CategoryDto>(cachedCategory);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }
            
            var request = new GetCategoryBySlugRequest() { Slug = slug };
            var result = await categoryProtoServiceClient.GetCategoryBySlugAsync(request);
            var data = mapper.Map<CategoryDto>(result);
            
            // Lưu cache
            var serializedData = serializeService.Serialize(data);
            await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
            });
            
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryBySlug), e);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto?>> GetAllNonStaticPageCategories()
    {
        try
        {
            var result = await categoryProtoServiceClient.GetAllNonStaticPageCategoriesAsync(new Empty());
            var data = mapper.Map<IEnumerable<CategoryDto>>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetAllNonStaticPageCategories), e);
            throw;
        }
    }
}