using Grpc.Core;
using Post.Domain.Interfaces;
using Post.GRPC.Protos;
using ILogger = Serilog.ILogger;

namespace Post.GRPC.Services;

public class PostService(IPostRepository postRepository, ILogger logger) : PostProtoService.PostProtoServiceBase
{
    public override async Task<HasPostsInCategoryResponse> HasPostsInCategory(HasPostsInCategoryRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(HasPostsInCategory);

        var result = new HasPostsInCategoryResponse() { Exists = false };

        try
        {
            result.Exists = await postRepository.HasPostsInCategory(request.CategoryId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            result.Exists = false;
        }

        return result;
    }
}