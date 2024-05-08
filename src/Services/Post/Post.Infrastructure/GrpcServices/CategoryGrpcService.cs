using Category.GRPC.Protos;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Dtos.Category;

namespace Post.Infrastructure.GrpcServices;

public class CategoryGrpcService(CategoryProtoService.CategoryProtoServiceClient categoryProtoServiceClient, ILogger logger) : ICategoryGrpcService
{
    public async Task<CategoryDto?> GetCategoryById(long id)
    {
        try
        {
            var categoryRequest = new GetCategoryRequest { Id = id };
            var result =  await categoryProtoServiceClient.GetCategoryByIdAsync(categoryRequest);
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
            logger.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            return null;
        }
    }
}