using AutoMapper;
using Grpc.Core;
using Post.Domain.Repositories;
using Post.Grpc.Protos;
using Shared.Constants;
using ILogger = Serilog.ILogger;

namespace Post.Grpc.Services;

public class PostService(IPostRepository postRepository, IMapper mapper, ILogger logger)
    : PostProtoService.PostProtoServiceBase
{
    public override async Task<HasPostsInCategoryResponse> HasPostsInCategory(HasPostsInCategoryRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(HasPostsInCategory);

        var result = new HasPostsInCategoryResponse();

        try
        {
            logger.Information("{MethodName} - Beginning to checking post belongs to the category id: {CategoryId}",
                methodName,
                request.CategoryId);

            result.Exists = await postRepository.HasPostsInCategory(request.CategoryId);

            logger.Information(
                "{MethodName} - Successfully checking post belongs to category id {CategoryId} with result {Result}.",
                methodName,
                request.CategoryId, result.Exists);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Exists = false;
        }

        return result;
    }

    public override async Task<GetPostsByIdsResponse> GetPostsByIds(GetPostsByIdsRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetPostsByIds);

        try
        {
            var postIds = request.Ids.Select(Guid.Parse).ToArray();

            logger.Information("{MethodName} - Beginning to retrieve posts for IDs: {PostIds}", methodName,
                postIds);

            var posts = await postRepository.GetPostsByIds(postIds);

            var data = mapper.Map<GetPostsByIdsResponse>(posts);

            logger.Information("{MethodName} - Successfully retrieved {Count} posts.", methodName, data.Posts.Count);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}