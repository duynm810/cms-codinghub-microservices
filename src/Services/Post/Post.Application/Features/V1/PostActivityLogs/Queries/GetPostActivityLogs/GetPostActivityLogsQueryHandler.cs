using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.PostActivityLogs.Queries.GetPostActivityLogs;

public class GetPostActivityLogsQueryHandler(IPostActivityLogRepository postActivityLogRepository, IMapper mapper, ILogger logger)
    : IRequestHandler<GetPostActivityLogsQuery, ApiResult<IEnumerable<PostActivityLogDto>>>
{
    public async Task<ApiResult<IEnumerable<PostActivityLogDto>>> Handle(GetPostActivityLogsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostActivityLogDto>>();

        try
        {
            var postActivityLogs = await postActivityLogRepository.GetActivityLogs(request.PostId);
            var data = mapper.Map<List<PostActivityLogDto>>(postActivityLogs);
            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostActivityLogsQuery), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}