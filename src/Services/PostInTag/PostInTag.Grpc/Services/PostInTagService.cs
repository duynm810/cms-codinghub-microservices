using Grpc.Core;
using PostInTag.Grpc.Protos;
using PostInTag.Grpc.Repositories.Interfaces;
using Shared.Constants;
using ILogger = Serilog.ILogger;

namespace PostInTag.Grpc.Services;

public class PostInTagService(IPostInTagRepository postInTagRepository, ILogger logger)
    : Protos.PostInTagService.PostInTagServiceBase
{
    public override async Task<GetTagsByPostIdResponse> GetTagsByPostId(GetTagsByPostIdRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetTagsByPostId);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving tag IDs for post with ID: {PostId}", methodName,
                request.PostId);

            var tagIds = await postInTagRepository.GetTagIdsByPostId(Guid.Parse(request.PostId));
            var tagIdList = tagIds.Select(id => id.ToString()).ToList();

            if (tagIdList.Count == 0)
            {
                logger.Warning("{MethodName} - No tags found for post with ID: {PostId}", methodName, request.PostId);
                return new GetTagsByPostIdResponse();
            }

            var response = new GetTagsByPostIdResponse();
            response.TagIds.AddRange(tagIdList);

            logger.Information("END {MethodName} - Successfully retrieved tag IDs for post with ID: {PostId}",
                methodName, request.PostId);

            return response;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx,
                "{MethodName}. RPC error occurred while getting tags for post with ID: {PostId}. Status: {StatusCode}, Detail: {Detail}",
                methodName, request.PostId, rpcEx.StatusCode, rpcEx.Status.Detail);
            throw;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}