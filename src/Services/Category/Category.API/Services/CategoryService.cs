using AutoMapper;
using Category.API.Entities;
using Category.API.GrpcServices.Interfaces;
using Category.API.Repositories.Interfaces;
using Category.API.Services.Interfaces;
using Shared.Dtos.Category;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Category.API.Services;

public class CategoryService(ICategoryRepository categoryRepository, IPostGrpcService postGrpcService, IMapper mapper, ILogger logger) : ICategoryService
{
    #region CRUD

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
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(CreateCategory), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<CategoryDto>> UpdateCategory(long id, UpdateCategoryDto model)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updateCategory = mapper.Map(model, category);
            await categoryRepository.UpdateCategory(updateCategory);

            var data = mapper.Map<CategoryDto>(updateCategory);
            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(UpdateCategory), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeleteCategory(List<long> ids)
    {
        var result = new ApiResult<bool>();

        try
        {
            foreach (var id in ids)
            {
                var category = await categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    result.Messages.Add("Category not found");
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var existedPost = await postGrpcService.HasPostsInCategory(id);
                if (existedPost)
                {
                    result.Messages.Add("The category contains post, cannot be deleted!");
                    result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                    return result;
                }

                await categoryRepository.DeleteCategory(category);
                result.Success(true);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(DeleteCategory), e);
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
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategories), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<CategoryDto>> GetCategoryById(long id)
    {
        var result = new ApiResult<CategoryDto>();

        try
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<CategoryDto>(category);
            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoryById), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion

    #region OTHERS

    public async Task<ApiResult<PagedResponse<CategoryDto>>> GetCategoriesPaging(int pageNumber, int pageSize)
    {
        var result = new ApiResult<PagedResponse<CategoryDto>>();

        try
        {
            var categories = await categoryRepository.GetCategoriesPaging(pageNumber, pageSize);
            var data = new PagedResponse<CategoryDto>()
            {
                Items = mapper.Map<List<CategoryDto>>(categories.Items),
                MetaData = categories.MetaData
            };

            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetCategoriesPaging), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion
}