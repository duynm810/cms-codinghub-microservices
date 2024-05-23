using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.SubmitPostForApproval;

public class SubmitPostForApprovalCommand(Guid id, Guid userId) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = userId;
}