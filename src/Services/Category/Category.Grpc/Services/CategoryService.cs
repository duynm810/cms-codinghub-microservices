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
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
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

            var categories = await categoryRepository.GetCategoryByIds(categoryIds);

            foreach (var category in categories)
            {
                result.Category.Add(new CategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug
                });
            }

            logger.Information("{MethodName} - Successfully retrieved {Count} categories.", methodName,
                result.Category.Count);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e.Message);
        }

        return result;
    }
}