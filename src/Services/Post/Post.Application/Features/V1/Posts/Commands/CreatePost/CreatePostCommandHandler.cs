using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Entities;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    ISerializeService serializeService,
    IPostEventService postEventService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<CreatePostCommand, ApiResult<Guid>>
{
    public async Task<ApiResult<Guid>> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Guid>();
        const string methodName = nameof(Handle);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating post with title: {Title}", methodName, command.Title);

            // Check slug exists
            var slugExists = await postRepository.SlugExists(command.Slug);
            if (slugExists)
            {
                logger.Warning("{MethodName} - Slug already exists: {PostSlug}", methodName, command.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.SlugExists);
                result.Failure(StatusCodes.Status409Conflict, result.Messages);
                return result;
            }

            // Check valid category id
            var category = await categoryGrpcClient.GetCategoryById(command.CategoryId);
            if (category == null)
            {
                logger.Warning("{MethodName} - Invalid category ID: {CategoryId}", methodName, command.CategoryId);
                result.Messages.Add(ErrorMessagesConsts.Category.InvalidCategoryId);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            var post = mapper.Map<PostBase>(command);

            // Set category id get by categories services
            post.CategoryId = category.Id;

            var id = postRepository.CreatePost(post);
            await postRepository.SaveChangesAsync();
            result.Success(id);

            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Post.GetAllPostsKey(),
                    CacheKeyHelper.Post.GetPinnedPostsKey(),
                    CacheKeyHelper.Post.GetFeaturedPostsKey(),
                    CacheKeyHelper.Post.GetMostLikedPostsKey(),
                    CacheKeyHelper.Post.GetMostCommentPostsKey(),
                    CacheKeyHelper.Post.GetPostByIdKey(id),
                    CacheKeyHelper.Post.GetPostBySlugKey(command.Slug),
                    CacheKeyHelper.Post.GetPostsByNonStaticPageCategoryKey()
                };

                await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });
            
            var rawTags = serializeService.Deserialize<List<RawTagDto>>(command.RawTags);
            if (rawTags != null)
            {
                TaskHelper.RunFireAndForget(() => postEventService.HandlePostCreatedEvent(id, rawTags), e =>
                {
                    logger.Error("HandlePostCreatedEvent failed. Message: {ErrorMessage}", e.Message);
                });
            }

            logger.Information("END {MethodName} - Post created successfully with ID: {PostId}", methodName, id);
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
