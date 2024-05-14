using Category.Grpc.Protos;
using Category.Grpc.Repositories.Interfaces;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace Category.Grpc.Services;

public class CategoryService(ICategoryRepository categoryRepository, ILogger logger)
    : CategoryProtoService.CategoryProtoServiceBase
{
    public override async Task<CategoryModel?> GetCategoryById(GetCategoryByIdRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetCategoryById);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting category by ID: {CategoryId}", methodName, request.Id);

            var category = await categoryRepository.GetCategoryById(request.Id);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found for ID: {CategoryId}", methodName, request.Id);
                return null;
            }

            var data = new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            };

            logger.Information(
                "END {MethodName} - Success: Retrieved Category {CategoryId} - Name: {CategoryName} - Slug: {CategorySlug}",
                methodName, data.Id, data.Name, data.Slug);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}. Error occurred while getting category by ID: {CategoryId}. Message: {ErrorMessage}", methodName, request.Id, e.Message);
            return null;
        }
    }

    public override async Task<GetCategoriesByIdsResponse> GetCategoriesByIds(GetCategoriesByIdsRequest request,
        ServerCallContext context)
    {
        var result = new GetCategoriesByIdsResponse();

        const string methodName = nameof(GetCategoriesByIds);

        try
        {
            var categoryIds = request.Ids.ToArray();

            logger.Information("{MethodName} - Beginning to retrieve categories for IDs: {CategoryIds}", methodName,
                categoryIds);

            var categories = await categoryRepository.GetCategoriesByIds(categoryIds);

            foreach (var category in categories)
            {
                result.Categories.Add(new CategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug
                });
            }

            logger.Information("{MethodName} - Successfully retrieved {Count} categories.", methodName,
                result.Categories.Count);
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}. Error occurred while getting categories by IDs: {CategoryIds}. Message: {ErrorMessage}", methodName, request.Ids, e.Message);
        }

        return result;
    }
}