using AutoMapper;
using Category.Grpc.Protos;
using Category.Grpc.Repositories.Interfaces;
using Grpc.Core;
using Shared.Constants;
using ILogger = Serilog.ILogger;

namespace Category.Grpc.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger logger)
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

            var data = mapper.Map<CategoryModel>(category);

            logger.Information(
                "END {MethodName} - Success: Retrieved Category {CategoryId} - Name: {CategoryName} - Slug: {CategorySlug}",
                methodName, data.Id, data.Name, data.Slug);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}. Error occurred while getting category by ID: {CategoryId}. Message: {ErrorMessage}",
                methodName, request.Id, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }

    public override async Task<GetCategoriesByIdsResponse> GetCategoriesByIds(GetCategoriesByIdsRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetCategoriesByIds);

        try
        {
            var categoryIds = request.Ids.ToArray();

            logger.Information("{MethodName} - Beginning to retrieve categories for IDs: {CategoryIds}", methodName,
                categoryIds);

            var categories = await categoryRepository.GetCategoriesByIds(categoryIds);

            var data = mapper.Map<GetCategoriesByIdsResponse>(categories);

            logger.Information("{MethodName} - Successfully retrieved {Count} categories.", methodName, data.Categories.Count);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting categories by IDs: {CategoryIds}. Message: {ErrorMessage}",
                methodName, request.Ids, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }

    public override async Task<CategoryModel> GetCategoryBySlug(GetCategoryBySlugRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetCategoryBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting category by Slug: {CategorySlug}", methodName, request.Slug);

            var category = await categoryRepository.GetCategoryBySlug(request.Slug);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found for Slug: {CategorySlug}", methodName, request.Slug);
                throw new RpcException(new Status(StatusCode.NotFound, $"Category with slug '{request.Slug}' not found."));
            }

            var data = mapper.Map<CategoryModel>(category);
        
            logger.Information(
                "END {MethodName} - Success: Retrieved Category {CategoryId} - Name: {CategoryName} - Slug: {CategorySlug}",
                methodName, data.Id, data.Name, data.Slug);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}. Error occurred while getting category by Slug: {CategorySlug}. Message: {ErrorMessage}", methodName, request.Slug, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}