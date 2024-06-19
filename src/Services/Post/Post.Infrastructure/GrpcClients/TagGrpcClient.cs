using AutoMapper;
using Contracts.Commons.Interfaces;
using Grpc.Core;
using Post.Domain.GrpcClients;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Helpers;
using Tag.Grpc.Protos;

namespace Post.Infrastructure.GrpcClients;

public class TagGrpcClient(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcClient
{
    public async Task<IEnumerable<TagDto>?> GetTagsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();
        
            var cacheKey = CacheKeyHelper.TagGrpc.GetGrpcTagsByIdsKey(idList);
            var cachedTags = await cacheService.GetAsync<IEnumerable<TagDto>>(cacheKey);
            if (cachedTags != null)
            {
                return cachedTags;
            }

            var request = new GetTagsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await tagProtoServiceClient.GetTagsByIdsAsync(request);
            if (result == null || result.Tags.Count == 0)
            {
                logger.Warning("{MethodName}: No tags found", methodName);
                return Enumerable.Empty<TagDto>();
            }
        
            var tagsByIds = mapper.Map<IEnumerable<TagDto>>(result);
            var data = tagsByIds.ToList();
        
            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}: gRPC error occurred while getting tags by ids. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, rpcEx.StatusCode, rpcEx.Message);
            return Enumerable.Empty<TagDto>();
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting tags by ids. Message: {ErrorMessage}", methodName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}