using AutoMapper;
using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using PostInTag.Api.GrpcClients.Interfaces;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.GrpcClients;

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

            var cacheKey = CacheKeyHelper.CategoryGrpc.GetGrpcCategoriesByIdsKey(idList);
            var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var request = new GetCategoriesByIdsRequest { Ids = { idList } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            if (result == null || result.Categories.Count == 0)
            {
                logger.Warning("{MethodName}: No categories found for the given ids", methodName);
                return Enumerable.Empty<CategoryDto>();
            }
        
            var categoriesByIds = mapper.Map<IEnumerable<CategoryDto>>(result);
            var data = categoriesByIds.ToList();
            await cacheService.SetAsync(cacheKey, data);
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting categories by ids. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            return Enumerable.Empty<CategoryDto>();
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting categories by ids. Message: {ErrorMessage}", methodName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}