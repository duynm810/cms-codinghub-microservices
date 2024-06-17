using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Post.Domain.GrpcClients;
using Serilog;
using Shared.Dtos.Category;
using Shared.Helpers;

namespace Post.Infrastructure.GrpcClients;

public class CategoryGrpcClient(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ICategoryGrpcClient
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        const string methodName = nameof(GetCategoryById);
        
        try
        {
            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoryByIdKey(id);
            var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var request = new GetCategoryByIdRequest { Id = id };
            var result = await categoryProtoServiceClient.GetCategoryByIdAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No category found by id {Id}", methodName, id);
                return null;
            }
            
            var data = mapper.Map<CategoryDto>(result);

            await cacheService.SetAsync(cacheKey, data);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        const string methodName = nameof(GetCategoriesByIds);

        try
        {
            var idList = ids as long[] ?? ids.ToArray();

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoriesByIdsKey(idList);
            var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var request = new GetCategoriesByIdsRequest() { Ids = { idList } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No categories found", methodName);
                return Enumerable.Empty<CategoryDto>();
            }
            
            var categoriesByIds = mapper.Map<IEnumerable<CategoryDto>>(result);

            var data = categoriesByIds.ToList();

            await cacheService.SetAsync(cacheKey, data);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public async Task<CategoryDto?> GetCategoryBySlug(string slug)
    {
        const string methodName = nameof(GetCategoryBySlug);
        
        try
        {
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoryBySlugKey(slug);

            // Check existed cache (Kiểm tra cache)
            var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var request = new GetCategoryBySlugRequest() { Slug = slug };
            var result = await categoryProtoServiceClient.GetCategoryBySlugAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No category found with slug {Slug}", methodName, slug);
                return null;
            }
            
            var data = mapper.Map<CategoryDto>(result);
            
            await cacheService.SetAsync(cacheKey, data);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetAllNonStaticPageCategories()
    {
        const string methodName = nameof(GetAllNonStaticPageCategories);
        
        try
        {
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcAllNonStaticPageCategoriesKey();

            // Check existed cache (Kiểm tra cache)
            var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var result = await categoryProtoServiceClient.GetAllNonStaticPageCategoriesAsync(new Empty());
            if (result == null)
            {
                logger.Warning("{MethodName}: No categories found", methodName);
                return Enumerable.Empty<CategoryDto>();
            }
            
            var allNonStaticPageCategories = mapper.Map<IEnumerable<CategoryDto>>(result);
            var data = allNonStaticPageCategories.ToList();

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