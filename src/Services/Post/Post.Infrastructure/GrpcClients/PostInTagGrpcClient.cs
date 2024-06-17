using Post.Domain.GrpcClients;
using PostInTag.Grpc.Protos;
using Serilog;

namespace Post.Infrastructure.GrpcClients;

public class PostInTagGrpcClient(PostInTagService.PostInTagServiceClient postInTagServiceClient, ILogger logger) : IPostInTagGrpcClient
{
    public async Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId)
    {
        const string methodName = nameof(GetTagIdsByPostIdAsync);

        try
        {
            var request = new GetTagsByPostIdRequest
            {
                PostId = postId.ToString()
            };

            var result = await postInTagServiceClient.GetTagsByPostIdAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No tag by post id {Id} not found", methodName, postId);
                return Enumerable.Empty<Guid>();
            }
            
            var tagIds = result.TagIds.Select(Guid.Parse);
            return tagIds;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }
}