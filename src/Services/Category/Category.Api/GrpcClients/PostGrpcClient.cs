using Category.Api.GrpcClients.Interfaces;
using Grpc.Core;
using Post.Grpc.Protos;
using Shared.Constants;
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
            var request = new HasPostsInCategoryRequest { CategoryId = categoryId };
            var result = await postProtoServiceClient.HasPostsInCategoryAsync(request);
            return result is { Exists: true };
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while checking posts in category {CategoryId}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, categoryId, rpcEx.StatusCode, rpcEx.Message);
            return false;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while checking posts in category {CategoryId}. Message: {ErrorMessage}", methodName, categoryId, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}