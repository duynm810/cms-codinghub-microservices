using AutoMapper;
using Contracts.Commons.Interfaces;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Shared.Requests.Tag;
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

    public async Task<ApiResult<TagDto>> CreateTag(CreateTagRequest request)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(CreateTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating tag with name: {TagName}", methodName, request.Name);

            var tag = mapper.Map<TagBase>(request);
            await tagRepository.CreateTag(tag);

            var data = mapper.Map<TagDto>(tag);
            result.Success(data);

            logger.Information("END {MethodName} - Tag created successfully with ID {TagId}", methodName, data.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<TagDto>> UpdateTag(Guid id, UpdateTagRequest request)
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

            var cacheKeys = new List<string>
            {
                CacheKeyHelper.Tag.GetTagByIdKey(id),
                CacheKeyHelper.Tag.GetTagBySlugKey(updateTag.Slug),
                CacheKeyHelper.Tag.GetTagByNameKey(updateTag.Name)
            };
            
            await cacheService.RemoveMultipleAsync(cacheKeys);

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
            
            var tasks = new List<Task>();

            foreach (var id in ids)
            {
                var tag = await tagRepository.GetTagById(id);
                if (tag == null)
                {
                    result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }
                
                tasks.Add(tagRepository.DeleteTag(tag));
                
                // Add cache removal tasks to the list (Thêm tác vụ xóa bộ nhớ cache vào danh sách)
                tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.Tag.GetTagByIdKey(id)));
            }
            
            await Task.WhenAll(tasks);
            
            result.Success(true);

            logger.Information("END {MethodName} - Tags with IDs {TagIds} deleted successfully", methodName, string.Join(", ", ids));
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
            
            var categories = await tagRepository.GetTags(count);
            if (categories.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<TagDto>>(categories);
                result.Success(data);
                
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

            var cacheKey = CacheKeyHelper.Tag.GetTagByIdKey(id);
            var cached = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cached != null)
            {
                result.Success(cached);
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

            var cacheKey = CacheKeyHelper.Tag.GetTagBySlugKey(slug);
            var cached = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cached != null)
            {
                result.Success(cached);
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
    
    public async Task<ApiResult<TagDto>> GetTagByName(string name)
    {
        var result = new ApiResult<TagDto>();
        const string methodName = nameof(GetTagByName);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving tag with name: {TagName}", methodName, name);

            var cacheKey = CacheKeyHelper.Tag.GetTagByNameKey(name);
            var cached = await cacheService.GetAsync<TagDto>(cacheKey);
            if (cached != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved tag with name {TagName} from cache", methodName, name);
                result.Success(cached);
                return result;
            }

            var tag = await tagRepository.GetTagByName(name);
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

            logger.Information("END {MethodName} - Successfully retrieved tag with name {TagName}", methodName, name);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
    
    public async Task<ApiResult<IEnumerable<TagDto>>> GetSuggestedTags(string? keyword, int count)
    {
        var result = new ApiResult<IEnumerable<TagDto>>();
        const string methodName = nameof(GetSuggestedTags);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving suggested tags with keyword {Keyword}", methodName, keyword);

            var cacheKey = CacheKeyHelper.Tag.GetSuggestedTagsKey(keyword, count);
            var cached = await cacheService.GetAsync<IEnumerable<TagDto>>(cacheKey);
            if (cached != null)
            {
                result.Success(cached);
                logger.Information("END {MethodName} - Successfully retrieved suggested tags from cache", methodName);
                return result;
            }

            var tags = await tagRepository.GetSuggestedTags(keyword, count);
            if (tags.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<TagDto>>(tags);
                result.Success(data);

                await cacheService.SetAsync(cacheKey, data);

                logger.Information("END {MethodName} - Successfully retrieved {TagCount} suggested tags with keyword {Keyword}", methodName, data.Count, keyword);
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

    #endregion
}