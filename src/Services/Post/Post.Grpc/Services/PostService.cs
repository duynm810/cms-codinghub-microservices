using Grpc.Core;
using Post.Domain.Repositories;
using Post.Grpc.Protos;
using ILogger = Serilog.ILogger;

namespace Post.Grpc.Services;

public class PostService(IPostRepository postRepository, ILogger logger) : PostProtoService.PostProtoServiceBase
{
    public override async Task<HasPostsInCategoryResponse> HasPostsInCategory(HasPostsInCategoryRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(HasPostsInCategory);

        var result = new HasPostsInCategoryResponse();

        try
        {
            logger.Information("{MethodName} - Beginning to checking post belongs to the category id: {CategoryId}", methodName,
                request.CategoryId);
            
            result.Exists = await postRepository.HasPostsInCategory(request.CategoryId);
            
            logger.Information("{MethodName} - Successfully checking post belongs to category id {CategoryId} with result {Result}.", methodName,
                request.CategoryId, result.Exists);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            result.Exists = false;
        }

        return result;
    }

    public override async Task<GetPostsByIdsResponse> GetPostsByIds(GetPostsByIdsRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetPostsByIds);
        
        var result = new GetPostsByIdsResponse();
        
        try
        {
            var postIds = request.Ids.Select(Guid.Parse).ToArray();
            
            logger.Information("{MethodName} - Beginning to retrieve posts for IDs: {PostIds}", methodName,
                postIds);

            var posts = await postRepository.GetPostsByIds(postIds);
            
            foreach (var post in posts)
            {
                result.Posts.Add(new PostModel
                {
                    Id = post.Id.ToString(),
                    Name = post.Name,
                    Slug = post.Slug
                });
            }
            
            logger.Information("{MethodName} - Successfully retrieved {Count} posts.", methodName,
                result.Posts.Count);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
        }

        return result;
    }
}