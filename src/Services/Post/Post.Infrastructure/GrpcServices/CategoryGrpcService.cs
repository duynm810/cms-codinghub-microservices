using AutoMapper;
using Category.Grpc.Protos;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Category;

namespace Post.Infrastructure.GrpcServices;

public class CategoryGrpcService(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    IMapper mapper,
    ILogger logger) : ICategoryGrpcService
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        try
        {
            var request = new GetCategoryByIdRequest { Id = id };
            var result = await categoryProtoServiceClient.GetCategoryByIdAsync(request);
            var data = mapper.Map<CategoryDto>(result);
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
        try
        {
            var request = new GetCategoriesByIdsRequest() { Ids = { ids } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            var data = mapper.Map<IEnumerable<CategoryDto>>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            throw;
        }
    }

    public async Task<CategoryDto?> GetCategoryBySlug(string slug)
    {
        try
        {
            var request = new GetCategoryBySlugRequest() { Slug = slug };
            var result = await categoryProtoServiceClient.GetCategoryBySlugAsync(request);
            var data = mapper.Map<CategoryDto>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryBySlug), e);
            throw;
        }
    }
}