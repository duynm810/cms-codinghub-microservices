using Post.Domain.GrpcServices;
using PostInTag.Grpc.Protos;
using Serilog;

namespace Post.Infrastructure.GrpcServices;

public class PostInTagGrpcService(PostInTagService.PostInTagServiceClient postInTagServiceClient, ILogger logger) : IPostInTagGrpcService
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