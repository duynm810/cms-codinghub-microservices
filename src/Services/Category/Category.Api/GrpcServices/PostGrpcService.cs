using Category.Api.GrpcServices.Interfaces;
using Post.GRPC.Protos;
using ILogger = Serilog.ILogger;

namespace Category.Api.GrpcServices;

public class PostGrpcService(PostProtoService.PostProtoServiceClient postProtoServiceClient, ILogger logger) : IPostGrpcService
{
    public async Task<bool> HasPostsInCategory(long categoryId)
    {
        try
        {
            var request = new HasPostsInCategoryRequest() { CategoryId = categoryId };
            var result = await postProtoServiceClient.HasPostsInCategoryAsync(request);
            return result.Exists;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(HasPostsInCategory), e);
            return false;
        }
    }
}