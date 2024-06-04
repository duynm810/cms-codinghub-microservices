using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.PostActivityLogs.Queries.GetPostActivityLogs;

public class GetPostActivityLogsQueryHandler(
    IPostActivityLogRepository postActivityLogRepository,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostActivityLogsQuery, ApiResult<IEnumerable<PostActivityLogModel>>>
{
    public async Task<ApiResult<IEnumerable<PostActivityLogModel>>> Handle(GetPostActivityLogsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostActivityLogModel>>();
        const string methodName = nameof(Handle);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving activity logs for post with ID: {PostId}", methodName,
                request.PostId);

            var postActivityLogs = await postActivityLogRepository.GetActivityLogs(request.PostId);
            var data = mapper.Map<List<PostActivityLogModel>>(postActivityLogs);
            result.Success(data);

            logger.Information(
                "END {MethodName} - Successfully retrieved {LogCount} activity logs for post with ID: {PostId}",
                methodName, data.Count, request.PostId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}