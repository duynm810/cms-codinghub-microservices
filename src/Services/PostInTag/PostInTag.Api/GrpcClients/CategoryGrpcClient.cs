using AutoMapper;
using Category.Grpc.Protos;
using Grpc.Core;
using PostInTag.Api.GrpcClients.Interfaces;
using Shared.Constants;
using Shared.Dtos.Category;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.GrpcClients;

public class CategoryGrpcClient(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    IMapper mapper,
    ILogger logger) : ICategoryGrpcClient
{
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        const string methodName = nameof(GetCategoriesByIds);

        try
        {
            var idList = ids as long[] ?? ids.ToArray();
            
            var request = new GetCategoriesByIdsRequest { Ids = { idList } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            if (result == null || result.Categories.Count == 0)
            {
                logger.Warning("{MethodName}: No categories found for the given ids", methodName);
                return Enumerable.Empty<CategoryDto>();
            }
        
            var categoriesByIds = mapper.Map<IEnumerable<CategoryDto>>(result);
            var data = categoriesByIds.ToList();
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