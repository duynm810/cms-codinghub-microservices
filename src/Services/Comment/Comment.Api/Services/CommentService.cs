using AutoMapper;
using Comment.Api.Entities;
using Comment.Api.Repositories.Interfaces;
using Comment.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.Comment;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Comment.Api.Services;

public class CommentService(ICommentRepository commentRepository, IMapper mapper, ILogger logger) : ICommentService
{
    public async Task<ApiResult<CommentDto>> CreateComment(CreateCommentDto request)
    {
        var result = new ApiResult<CommentDto>();
        const string methodName = nameof(CreateComment);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating comment with content: {Content}", methodName,
                request.Content);

            var comment = mapper.Map<CommentBase>(request);
            await commentRepository.CreateComment(comment);

            var data = mapper.Map<CommentDto>(comment);
            result.Success(data);

            logger.Information("END {MethodName} - Comment created successfully with ID {CommentId}", methodName,
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

    public async Task<ApiResult<IEnumerable<CommentDto>>> GetCommentsByPostId(Guid postId)
    {
        var result = new ApiResult<IEnumerable<CommentDto>>();
        const string methodName = nameof(GetCommentsByPostId);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving comment with post ID: {PostId}", methodName, postId);

            var comments = await commentRepository.GetCommentsByPostId(postId);
            var commentList = mapper.Map<IEnumerable<CommentDto>>(comments);

            var data = BuildCommentTree(commentList.ToList());

            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved comment with post ID: {PostId}", methodName,
                postId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> LikeComment(string commentId)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(LikeComment);

        try
        {
            logger.Information("BEGIN {MethodName} - Liking comment with ID: {CommentId}", methodName, commentId);

            var success = await commentRepository.UpdateLikeCount(commentId, 1);
            result.Success(success);

            logger.Information("END {MethodName} - Successfully liked comment with ID: {CommentId}", methodName,
                commentId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
    
    public async Task<ApiResult<bool>> ReplyToComment(string parentId, CreateCommentDto newCommentDto)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(ReplyToComment);

        try
        {
            logger.Information("BEGIN {MethodName} - Replying to comment with ID: {ParentId}", methodName, parentId);

            var parentComment = await commentRepository.GetCommentById(parentId);
            if (parentComment == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Comment.ParentIdNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                logger.Warning("{MethodName} - Parent comment with ID: {ParentId} not found.", methodName, parentId);
                return result;
            }
            
            var newComment = mapper.Map<CommentBase>(newCommentDto);
            newComment.ParentId = parentId;
          
            var created = await commentRepository.CreateComment(newComment);
            if (!created)
            {
                result.Messages.Add(ErrorMessagesConsts.Comment.CommentCreationFailed);
                result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
                logger.Error("{MethodName} - Failed to create new comment.", methodName);
                return result;
            }

            var updated = await commentRepository.UpdateRepliesCount(parentId, 1);
            if (!updated)
            {
                result.Messages.Add(ErrorMessagesConsts.Comment.RepliesCountUpdateFailed);
                result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
                logger.Error("{MethodName} - Failed to update replies count for comment with ID: {ParentId}", methodName, parentId);
                return result;
            }
            
            result.Success(true);
            logger.Information("END {MethodName} - Successfully replied to comment with ID: {ParentId}", methodName, parentId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    private List<CommentDto> BuildCommentTree(List<CommentDto> comments)
    {
        var commentDictionary = comments.ToDictionary(c => c.Id);
        var treeComments = new List<CommentDto>();

        foreach (var comment in comments)
        {
            if (comment.ParentId == null)
            {
                treeComments.Add(comment);
            }
            else if (commentDictionary.TryGetValue(comment.ParentId, out var parent))
            {
                parent.Replies ??= [];
                parent.Replies.Add(comment);
            }
        }

        return treeComments;
    }
}