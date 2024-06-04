using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Category;
using Shared.Helpers;

namespace Post.Infrastructure.GrpcServices;

public class CategoryGrpcService(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    IMapper mapper,
    ICacheService cacheService,
    ILogger logger) : ICategoryGrpcService
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        try
        {
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoryByIdKey(id);
            var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var request = new GetCategoryByIdRequest { Id = id };
            var result = await categoryProtoServiceClient.GetCategoryByIdAsync(request);
            var data = mapper.Map<CategoryDto>(result);

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

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
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoriesByIdsKey(idList);
            var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var request = new GetCategoriesByIdsRequest() { Ids = { idList } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            var categoriesByIds = mapper.Map<IEnumerable<CategoryDto>>(result);

            var data = categoriesByIds.ToList();

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
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
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoryBySlugKey(slug);

            // Kiểm tra cache
            var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var request = new GetCategoryBySlugRequest() { Slug = slug };
            var result = await categoryProtoServiceClient.GetCategoryBySlugAsync(request);
            var data = mapper.Map<CategoryDto>(result);

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

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
            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcAllNonStaticPageCategoriesKey();

            // Kiểm tra cache
            var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var result = await categoryProtoServiceClient.GetAllNonStaticPageCategoriesAsync(new Empty());
            var allNonStaticPageCategories = mapper.Map<IEnumerable<CategoryDto>>(result);

            var data = allNonStaticPageCategories.ToList();

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetAllNonStaticPageCategories), e);
            throw;
        }
    }
}