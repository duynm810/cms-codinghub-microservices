using Infrastructure.Paged;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post.Queries;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByTagPaging;

public class GetPostsByTagPagingQueryHandler(
    IPostRepository postRepository,
    IPostInTagGrpcClient postInTagGrpcClient,
    ITagGrpcClient tagGrpcClient,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetPostsByTagPagingQuery, ApiResult<PostsByTagDto>>
{
    public async Task<ApiResult<PostsByTagDto>> Handle(GetPostsByTagPagingQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsByTagDto>();
        const string methodName = nameof(GetPostsByTagPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts by tag with Slug: {TagSlug} for page {PageNumber} with page size {PageSize}", methodName, query.TagSlug, query.Request.PageNumber, query.Request.PageSize);
            
            var tag = await tagGrpcClient.GetTagBySlug(query.TagSlug);
            if (tag == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Tag.TagNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var postIds = await postInTagGrpcClient.GetPostIdsInTagAsync(tag.Id);
            var postIdList = postIds.ToArray();
            if (!postIdList.IsNotNullOrEmpty())
            {
                result.Messages.Add(ErrorMessagesConsts.PostInTag.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
           
            var posts = await postRepository.GetPostsByIds(postIdList);
            var postList = posts.ToList();
            if (!postList.IsNotNullOrEmpty())
            {
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
                
            var enrichedPosts = await postService.EnrichPostsWithCategories(postList, cancellationToken);
                        
            var items = PagedList<PostDto>.ToPagedList(enrichedPosts, query.Request.PageNumber, query.Request.PageSize, x => x.Id);

            var data = new PostsByTagDto()
            {
                Tag = tag,
                Posts = new PagedResponse<PostDto>()
                {
                    Items = items,
                    MetaData = items.GetMetaData()
                }
            };
                        
            result.Success(data);
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