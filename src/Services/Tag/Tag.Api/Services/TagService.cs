using AutoMapper;
using Contracts.Commons.Interfaces;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;
using Tag.Api.Entities;
using Tag.Api.Repositories.Interfaces;
using Tag.Api.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Services;

public class TagService(ITagRepository tagRepository, ICacheService cacheService, IMapper mapper, ILogger logger)
    : ITagService
{
    #region CRUD

    public async Task<ApiResult<TagDto>> CreateTag(CreateTagDto request)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(CreateTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating tag with name: {TagName}", methodName,
                request.Name);

            var tag = mapper.Map<TagBase>(request);
            await tagRepository.CreateTag(tag);

            var data = mapper.Map<TagDto>(tag);
            result.Success(data);

            // Xóa cache danh sách tag khi tạo mới
            await cacheService.RemoveAsync(CacheKeyHelper.Tag.GetAllTagsKey());
            await cacheService.RemoveAsync(CacheKeyHelper.TagGrpc.GetAllTagsKey());

            logger.Information("END {MethodName} - Tag created successfully with ID {TagId}", methodName,
                data.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<TagDto>> UpdateTag(Guid id, UpdateTagDto request)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(UpdateTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating tag with ID: {TagId}", methodName, id);

            var tag = await tagRepository.GetTagById(id);
            if (tag == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updateTag = mapper.Map(request, tag);
            await tagRepository.UpdateTag(updateTag);

            var data = mapper.Map<TagDto>(updateTag);
            result.Success(data);

            // Delete tag list cache when updating (Xóa cache danh sách tag khi cập nhật)
            await cacheService.RemoveAsync(CacheKeyHelper.Tag.GetAllTagsKey());
            await cacheService.RemoveAsync(CacheKeyHelper.Tag.GetTagByIdKey(id));
            await cacheService.RemoveAsync(CacheKeyHelper.TagGrpc.GetAllTagsKey());

            logger.Information("END {MethodName} - Tag with ID {TagId} updated successfully", methodName, id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeleteTag(List<Guid> ids)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeleteTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting categories with IDs: {TagIds}", methodName,
                string.Join(", ", ids));

            foreach (var id in ids)
            {
                var tag = await tagRepository.GetTagById(id);
                if (tag == null)
                {
                    result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                await tagRepository.DeleteTag(tag);

                // Delete tag cache when delete (Xóa cache tag khi xoá dữ liệu)
                await cacheService.RemoveAsync(CacheKeyHelper.Tag.GetTagByIdKey(id));
            }

            // Delete tag list cache when deleting (Xóa cache danh sách tag khi xóa dữ liệu)
            await cacheService.RemoveAsync(CacheKeyHelper.Tag.GetAllTagsKey());
            await cacheService.RemoveAsync(CacheKeyHelper.TagGrpc.GetAllTagsKey());

            result.Success(true);

            logger.Information("END {MethodName} - Tags with IDs {TagIds} deleted successfully",
                methodName, string.Join(", ", ids));
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetTags(int count)
    {
        var result = new ApiResult<IEnumerable<TagDto>>();
        const string methodName = nameof(GetTags);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all tags", methodName);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Tag.GetAllTagsKey();
            var cachedTags = await cacheService.GetAsync<IEnumerable<TagDto>>(cacheKey);
            if (cachedTags != null)
            {
                result.Success(cachedTags);
                logger.Information("END {MethodName} - Successfully retrieved tags from cache", methodName);
                return result;
            }

            var categories = await tagRepository.GetTags(count);
            if (categories.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<TagDto>>(categories);
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information("END {MethodName} - Successfully retrieved {TagCount} tags", methodName,
                    data.Count);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<TagDto>> GetTagById(Guid id)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(GetTagById);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving tag with ID: {TagId}", methodName, id);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Tag.GetTagByIdKey(id);
            var cachedTag = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cachedTag != null)
            {
                result.Success(cachedTag);
                logger.Information("END {MethodName} - Successfully retrieved tag with ID {TagId} from cache",
                    methodName, id);
                return result;
            }

            var tag = await tagRepository.GetTagById(id);
            if (tag == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<TagDto>(tag);
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            logger.Information("END {MethodName} - Successfully retrieved tag with ID {TagId}", methodName,
                id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion

    #region OTHERS

    public async Task<ApiResult<TagDto>> GetTagBySlug(string slug)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(GetTagBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving tag with slug: {TagSlug}", methodName, slug);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Tag.GetTagBySlugKey(slug);
            var cachedTag = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cachedTag != null)
            {
                result.Success(cachedTag);
                logger.Information(
                    "END {MethodName} - Successfully retrieved tag with slug {TagSlug} from cache",
                    methodName, slug);
                return result;
            }

            var tag = await tagRepository.GetTagBySlug(slug);
            if (tag == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<TagDto>(tag);
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            logger.Information("END {MethodName} - Successfully retrieved tag with slug {TagSlug}",
                methodName,
                slug);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion
}