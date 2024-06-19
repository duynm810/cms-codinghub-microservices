using AutoMapper;
using Contracts.Commons.Interfaces;
using Infrastructure.Paged;
using PostInTag.Api.Entities;
using PostInTag.Api.GrpcClients.Interfaces;
using PostInTag.Api.Repositories.Interfaces;
using PostInTag.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.PostInTag;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Services;

public class PostInTagService(
    IPostInTagRepository postInTagRepository,
    IPostGrpcClient postGrpcClient,
    ITagGrpcClient tagGrpcClient,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IPostInTagService
{
    public async Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagDto request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(CreatePostToTag);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating post with ID: {PostId} to tag with ID: {TagId} with sort order: {SortOrder}",
                methodName, request.PostId, request.TagId, request.SortOrder);

            var postInTag = mapper.Map<PostInTagBase>(request);

            await postInTagRepository.CreatePostToTag(postInTag);
            result.Success(true);

            // Xoá cache
            await cacheService.RemoveAsync(CacheKeyHelper.PostInTag.GetAllPostInTagByIdKey(request.TagId));

            logger.Information(
                "END {MethodName} - Successfully created post with ID: {PostId} to tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagDto request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePostToTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId} from tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);

            var postInTag = mapper.Map<PostInTagBase>(request);

            await postInTagRepository.DeletePostToTag(postInTag);
            result.Success(true);
            
            // Xoá cache
            await cacheService.RemoveAsync(CacheKeyHelper.PostInTag.GetAllPostInTagByIdKey(request.TagId));

            logger.Information(
                "END {MethodName} - Successfully deleted post with ID: {PostId} from tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTag(Guid tagId)
    {
        var result = new ApiResult<IEnumerable<PostInTagDto>>();
        const string methodName = nameof(GetPostsInTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts in tag with ID: {TagId}", methodName, tagId);
            
            var cacheKey = CacheKeyHelper.PostInTag.GetAllPostInTagByIdKey(tagId);
            var cachedPostInTag = await cacheService.GetAsync<IEnumerable<PostInTagDto>>(cacheKey);
            if (cachedPostInTag != null)
            {
                result.Success(cachedPostInTag);
                logger.Information("END {MethodName} - Successfully retrieved post in tag with ID {TagId} from cache", methodName, tagId);
                return result;
            }

            var postIds = await postInTagRepository.GetPostIdsInTag(tagId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for tag with ID: {TagId}", methodName, tagId);
                result.Messages.Add(ErrorMessagesConsts.PostInTag.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Count != 0)
            {
                var postInTagDtos = await postGrpcClient.GetPostsByIds(postList);
                var data = postInTagDtos.ToList();
                result.Success(data);
                
                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for tag with ID: {TagId}",
                    methodName, data.Count, tagId);
            }
            else
            {
                result.Messages.Add(ErrorMessagesConsts.PostInTag.PostNotFoundInTag);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
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

    public async Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTagBySlug(string tagSlug)
    {
        var result = new ApiResult<IEnumerable<PostInTagDto>>();
        const string methodName = nameof(GetPostsInTagBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts in tag with Slug: {TagSlug}", methodName, tagSlug);

            var cacheKey = CacheKeyHelper.PostInTag.GetPostInTagBySlugKey(tagSlug);
            var cachedPostInTag = await cacheService.GetAsync<IEnumerable<PostInTagDto>>(cacheKey);
            if (cachedPostInTag != null)
            {
                result.Success(cachedPostInTag);
                logger.Information("END {MethodName} - Successfully retrieved post in tag with Slug {TagSlug} from cache", methodName, tagSlug);
                return result;
            }
            
            var tag = await tagGrpcClient.GetTagBySlug(tagSlug);
            if (tag != null)
            {
                var postIds = await postInTagRepository.GetPostIdsInTag(tag.Id);
                if (postIds == null)
                {
                    logger.Warning("{MethodName} - Post IDs not found for tag with Slug: {TagSlug}", methodName,
                        tag.Slug);
                    result.Messages.Add(ErrorMessagesConsts.PostInTag.PostIdsNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var postList = postIds.ToList();
                if (postList.Count != 0)
                {
                    var postInTagDtos = await postGrpcClient.GetPostsByIds(postList);
                    var data = postInTagDtos.ToList();
                    result.Success(data);
                    
                    // Save cache (Lưu cache)
                    await cacheService.SetAsync(cacheKey, data);

                    logger.Information(
                        "END {MethodName} - Successfully retrieved {PostCount} posts for tag with Slug: {TagSlug}",
                        methodName, data.Count, tag.Slug);
                }
                else
                {
                    result.Messages.Add(ErrorMessagesConsts.PostInTag.PostNotFoundInTag);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                }
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

    public async Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagPaging(Guid tagId, int pageNumber, int pageSize)
    {
        var result = new ApiResult<PagedResponse<PostInTagDto>>();
        const string methodName = nameof(GetPostsInTagPaging);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts in tag with ID: {TagId} for page {PageNumber} with page size {PageSize}",
                methodName, tagId, pageNumber, pageSize);
            
            var cacheKey = CacheKeyHelper.PostInTag.GetPostInTagByIdPagingKey(tagId, pageNumber, pageSize);
            var cachedPostInTag = await cacheService.GetAsync<PagedResponse<PostInTagDto>>(cacheKey);
            if (cachedPostInTag != null)
            {
                result.Success(cachedPostInTag);
                logger.Information("END {MethodName} - Successfully retrieved post in tag with ID {TagId} from cache", methodName, tagId);
                return result;
            }

            var postIds = await postInTagRepository.GetPostIdsInTag(tagId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for tag with ID: {TagId}", methodName, tagId);
                result.Messages.Add(ErrorMessagesConsts.PostInTag.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Any())
            {
                var posts = await postGrpcClient.GetPostsByIds(postList);
                var items = PagedList<PostInTagDto>.ToPagedList(posts, pageNumber, pageSize, x => x.Id);

                var data = new PagedResponse<PostInTagDto>
                {
                    Items = items,
                    MetaData = items.GetMetaData()
                };

                result.Success(data);
                
                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for tag with ID: {TagId} for page {PageNumber} with page size {PageSize}",
                    methodName, items.Count, tagId, pageNumber, pageSize);
            }
            else
            {
                result.Messages.Add(ErrorMessagesConsts.PostInTag.PostNotFoundInTag);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
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

    public async Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagBySlugPaging(string tagSlug, int pageNumber, int pageSize)
    {
        var result = new ApiResult<PagedResponse<PostInTagDto>>();
        const string methodName = nameof(GetPostsInTagBySlugPaging);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts in tag with Slug: {TagSlug} for page {PageNumber} with page size {PageSize}",
                methodName, tagSlug, pageNumber, pageSize);
            
            var cacheKey = CacheKeyHelper.PostInTag.GetPostInTagBySlugPagingKey(tagSlug, pageNumber, pageSize);
            var cachedPostInTag = await cacheService.GetAsync<PagedResponse<PostInTagDto>>(cacheKey);
            if (cachedPostInTag != null)
            {
                result.Success(cachedPostInTag);
                logger.Information("END {MethodName} - Successfully retrieved post in tag with Slug {TagSlug} from cache", methodName, tagSlug);
                return result;
            }

            var tag = await tagGrpcClient.GetTagBySlug(tagSlug);
            if (tag != null)
            {
                var postIds = await postInTagRepository.GetPostIdsInTag(tag.Id);
                if (postIds == null)
                {
                    logger.Warning("{MethodName} - Post IDs not found for tag with Slug: {TagSlug}", methodName,
                        tag.Slug);
                    result.Messages.Add(ErrorMessagesConsts.PostInTag.PostIdsNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var postIdList = postIds.ToList();
                if (postIdList.Any())
                {
                    var posts = await postGrpcClient.GetPostsByIds(postIdList);

                    var postList = posts.ToList();

                    var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                    var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);
                    var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                    foreach (var post in postList)
                    {
                        if (categoryDictionary.TryGetValue(post.CategoryId, out var category))
                        {
                            mapper.Map(category, post);
                        }
                    }

                    var items = PagedList<PostInTagDto>.ToPagedList(postList, pageNumber, pageSize, x => x.Id);

                    var data = new PagedResponse<PostInTagDto>
                    {
                        Items = items,
                        MetaData = items.GetMetaData()
                    };

                    result.Success(data);
                    
                    // Save cache (Lưu cache)
                    await cacheService.SetAsync(cacheKey, data);

                    logger.Information(
                        "END {MethodName} - Successfully retrieved {PostCount} posts for tag with Slug: {TagSlug} for page {PageNumber} with page size {PageSize}",
                        methodName, items.Count, tag.Slug, pageNumber, pageSize);
                }
                else
                {
                    result.Messages.Add(ErrorMessagesConsts.PostInTag.PostNotFoundInTag);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                }
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
}