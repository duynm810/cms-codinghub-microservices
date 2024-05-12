using AutoMapper;
using Post.Grpc.Protos;
using PostInSeries.Api.GrpcServices.Interfaces;
using Shared.Dtos.PostInSeries;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.GrpcServices;

public class PostGrpcService(PostProtoService.PostProtoServiceClient postProtoServiceClient, IMapper mapper, ILogger logger)
    : IPostGrpcService
{
    public async Task<IEnumerable<PostInSeriesDto>> GetPostsByIds(IEnumerable<Guid> ids)
    {
        try
        {
            // Convert each GUID to its string representation
            var request = new GetPostsByIdsRequest();
            request.Ids.AddRange(ids.Select(id => id.ToString()));

            var result = await postProtoServiceClient.GetPostsByIdsAsync(request);
            if (result != null && result.Posts.Count != 0)
            {
                return mapper.Map<IEnumerable<PostInSeriesDto>>(result.Posts);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsByIds), e);
        }

        return new List<PostInSeriesDto>();
    }
}