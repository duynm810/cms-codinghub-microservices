using AutoMapper;
using Comment.Api.Entities;
using Comment.Api.Repositories.Interfaces;
using Comment.Api.Services.Interfaces;
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