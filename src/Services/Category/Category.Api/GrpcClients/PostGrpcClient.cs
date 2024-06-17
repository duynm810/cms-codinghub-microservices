using Category.Api.GrpcClients.Interfaces;
using Post.Grpc.Protos;
using ILogger = Serilog.ILogger;

namespace Category.Api.GrpcClients;

public class PostGrpcClient(PostProtoService.PostProtoServiceClient postProtoServiceClient, ILogger logger)
    : IPostGrpcClient
{
    public async Task<bool> HasPostsInCategory(long categoryId)
    {
        const string methodName = nameof(HasPostsInCategory);
        
        try
        {
            var request = new HasPostsInCategoryRequest() { CategoryId = categoryId };
            var result = await postProtoServiceClient.HasPostsInCategoryAsync(request);
            return result is { Exists: true };
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            return false;
        }
    }
}