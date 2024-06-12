using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using PostInSeries.Api.GrpcClients.Interfaces;
using Shared.Dtos.Category;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcClients;

public class CategoryGrpcClient(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ICategoryGrpcClient
{
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
            var categoriesByIds = mapper.Map<IEnumerable<CategoryDto>>(result);

            var data = categoriesByIds.ToList();

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