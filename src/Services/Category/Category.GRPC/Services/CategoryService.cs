using Category.GRPC.Protos;
using Category.GRPC.Repositories.Interfaces;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace Category.GRPC.Services;

public class CategoryService(ICategoryRepository categoryRepository, ILogger logger) : CategoryProtoService.CategoryProtoServiceBase
{
    public override async Task<CategoryModel?> GetCategoryById(GetCategoryRequest request, ServerCallContext context)
    {
        try
        {
            logger.Information("BEGIN Get Category By Id function : {CategoryId}", request.Id);

            var category = await categoryRepository.GetCategoryById(request.Id);
            if (category == null)
            {
                logger.Information("END Get Category By Id with result: {CategoryId} not found", request.Id);
                return null;
            }
        
            var data = new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            };

            logger.Information("END Get Category By Id with result: {CategoryId} - Name {Name} - Slug {Slug}", request.Id,
                data.Name, data.Slug);
        
            return data;

        }
        catch (Exception e)
        {
            logger.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e.Message);
            return null;
        }
    }
}