using AutoMapper;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using Post.Grpc.Protos;
using PostInSeries.Api.GrpcClients.Interfaces;
using Shared.Constants;
using Shared.Dtos.PostInSeries;
using Shared.Helpers;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcClients;

public class PostGrpcClient(
    PostProtoService.PostProtoServiceClient postProtoServiceClient,
    IMapper mapper,
    ILogger logger)
    : IPostGrpcClient
{
    public async Task<IEnumerable<PostInSeriesDto>> GetPostsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetPostsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();

            // Convert each GUID to its string representation
            var request = new GetPostsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await postProtoServiceClient.GetPostsByIdsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                var postsByIds = mapper.Map<IEnumerable<PostInSeriesDto>>(result.Posts);
                var data = postsByIds.ToList();
                return data;
            }

            logger.Warning("{MethodName}: No posts found for the given ids", methodName);
            return Enumerable.Empty<PostInSeriesDto>();
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting posts by ids. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting posts by ids. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }
}