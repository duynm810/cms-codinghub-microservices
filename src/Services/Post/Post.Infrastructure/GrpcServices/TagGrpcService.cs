using AutoMapper;
using Contracts.Commons.Interfaces;
using Post.Domain.GrpcServices;
using Serilog;
using Shared.Dtos.Tag;
using Tag.Grpc.Protos;

namespace Post.Infrastructure.GrpcServices;

public class TagGrpcService(
    TagProtoService.TagProtoServiceClient tagProtoServiceClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ITagGrpcService
{
    public async Task<IEnumerable<TagDto>> GetTagsByIds(IEnumerable<Guid> ids)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var idList = ids as Guid[] ?? ids.ToArray();

            var request = new GetTagsByIdsRequest();
            request.Ids.AddRange(idList.Select(id => id.ToString()));

            var result = await tagProtoServiceClient.GetTagsByIdsAsync(request);
            var tagsByIds = mapper.Map<IEnumerable<TagDto>>(result);

            var data = tagsByIds.ToList();

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}