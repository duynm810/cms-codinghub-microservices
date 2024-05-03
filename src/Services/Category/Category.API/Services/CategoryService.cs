using AutoMapper;
using Category.API.Repositories.Interfaces;
using Category.API.Services.Interfaces;
using Serilog;
using Shared.Dtos.Category;
using Shared.Responses;
using Shared.Utilities;

namespace Category.API.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapperBase mapper) : ICategoryService
{
    public async Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var result = new ApiResult<IEnumerable<CategoryDto>>();
        
        try
        {
            var categories = await categoryRepository.GetCategories();
            if (categories.IsNotNullOrEmpty())
            {
                var data = mapper.Map<IEnumerable<CategoryDto>>(categories);
                result.Success(data);
            }
        }
        catch (Exception e)
        {
            Log.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(GetCategories), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}