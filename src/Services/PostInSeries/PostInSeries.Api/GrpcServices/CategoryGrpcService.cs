using AutoMapper;
using Category.Grpc.Protos;
using PostInSeries.Api.GrpcServices.Interfaces;
using Shared.Dtos.Category;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class CategoryGrpcService(CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient, IMapper mapper, ILogger logger) : ICategoryGrpcService
{
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        try
        {
            var request = new GetCategoriesByIdsRequest() { Ids = { ids } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            var data = mapper.Map<IEnumerable<CategoryDto>>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoriesByIds), e);
            throw;
        }
    }
}