using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using PostInSeries.Api.GrpcServices.Interfaces;
using Shared.Dtos.Category;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class CategoryGrpcService(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ICategoryGrpcService
{
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        const string methodName = nameof(GetCategoriesByIds);
        
        try
        {
            var idList = ids as long[] ?? ids.ToArray();

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
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}