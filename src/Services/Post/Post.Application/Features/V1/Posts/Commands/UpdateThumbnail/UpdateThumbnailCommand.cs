using MediatR;
using Shared.Requests.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.UpdateThumbnail;

public class UpdateThumbnailCommand : UpdateThumbnailRequest, IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; }

    public void SetId(Guid id)
    {
        Id = id;
    }
}