using MediatR;
using Shared.Requests.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(RejectPostWithReasonRequest request, Guid id, Guid userId)
    : IRequest<ApiResult<bool>>
{
    public RejectPostWithReasonRequest Request { get; set; } = request;
    
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = userId;
}