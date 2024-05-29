using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.PostActivityLogs.Queries.GetPostActivityLogs;

public class GetPostActivityLogsQuery(Guid postId)  : IRequest<ApiResult<IEnumerable<PostActivityLogModel>>>
{
    public Guid PostId { get; set; } = postId;
}