using Grpc.Core;
using Post.Domain.Repositories;
using Post.GRPC.Protos;
using ILogger = Serilog.ILogger;

namespace Post.GRPC.Services;

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
}