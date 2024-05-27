using Category.Grpc.Protos;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Category;

namespace Post.Infrastructure.GrpcServices;

public class CategoryGrpcService(
    CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient,
    ILogger logger) : ICategoryGrpcService
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        try
        {
            var request = new GetCategoryByIdRequest { Id = id };
            var result = await categoryProtoServiceClient.GetCategoryByIdAsync(request);
            if (result != null)
            {
                return new CategoryDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Slug = result.Slug,
                    Icon = result.Icon,
                    Color = result.Color
                };
            }

            return null;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            return null;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids)
    {
        try
        {
            var request = new GetCategoriesByIdsRequest() { Ids = { ids } };
            var result = await categoryProtoServiceClient.GetCategoriesByIdsAsync(request);
            if (result != null)
            {
                return result.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Icon = c.Icon,
                    Color = c.Color
                }).ToList();
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
        }

        return [];
    }

    public async Task<CategoryDto?> GetCategoryBySlug(string slug)
    {
        try
        {
            var request = new GetCategoryBySlugRequest() { Slug = slug };
            var result = await categoryProtoServiceClient.GetCategoryBySlugAsync(request);
            if (result != null)
            {
                return new CategoryDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Slug = result.Slug
                };
            }

            return null;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryBySlug), e);
            return null;
        }
    }
}