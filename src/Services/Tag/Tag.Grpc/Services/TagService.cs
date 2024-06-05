using AutoMapper;
using Grpc.Core;
using Tag.Grpc.Protos;
using Tag.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Grpc.Services;

public class TagService(ITagRepository tagRepository, IMapper mapper, ILogger logger)
    : TagProtoService.TagProtoServiceBase
{
    public override async Task<GetTagsByIdsResponse> GetTagsByIds(GetTagsByIdsRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var tagsIds = request.Ids.Select(Guid.Parse).ToArray();

            logger.Information("{MethodName} - Beginning to retrieve tags for IDs: {TagsIds}", methodName,
                tagsIds);

            var tags = await tagRepository.GetTagsByIds(tagsIds);

            var data = mapper.Map<GetTagsByIdsResponse>(tags);

            return data;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting tags by IDs: {CategoryIds}. Message: {ErrorMessage}",
                methodName, request.Ids, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting tags by IDs"));
        }
    }
}