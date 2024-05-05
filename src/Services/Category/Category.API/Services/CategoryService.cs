using AutoMapper;
using Category.API.Entities;
using Category.API.Repositories.Interfaces;
using Category.API.Services.Interfaces;
using Serilog;
using Shared.Dtos.Category;
using Shared.Responses;
using Shared.Utilities;

namespace Category.API.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    public async Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryDto model)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            if (string.IsNullOrEmpty(model.Slug))
            {
                model.Slug = Utils.ToUnSignString(model.Name);
            }

            var category = mapper.Map<CategoryBase>(model);
            await categoryRepository.CreateCategory(category);

            var data = mapper.Map<CategoryDto>(category);
            result.Success(data);
        }
        catch (Exception e)
        {
            Log.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(CreateCategory), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<CategoryDto>> UpdateCategory(Guid id, UpdateCategoryDto model)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                return result;
            }

            var updateCategory = mapper.Map(model, category);
            await categoryRepository.UpdateCategory(updateCategory);

            var data = mapper.Map<CategoryDto>(updateCategory);
            result.Success(data);
        }
        catch (Exception e)
        {
            Log.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(UpdateCategory), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<CategoryDto>> DeleteCategory(Guid id)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                return result;
            }

            await categoryRepository.DeleteCategory(category);

            var data = mapper.Map<CategoryDto>(category);
            result.Success(data);
        }
        catch (Exception e)
        {
            Log.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(DeleteCategory), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

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
    
    public async Task<ApiResult<CategoryDto>> GetCategoryById(Guid id)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                return result;
            }

            var data = mapper.Map<CategoryDto>(category);
            result.Success(data);
        }
        catch (Exception e)
        {
            Log.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}