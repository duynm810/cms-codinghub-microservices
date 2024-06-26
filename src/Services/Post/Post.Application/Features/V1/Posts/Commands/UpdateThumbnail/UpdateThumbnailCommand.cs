using MediatR;
using Shared.Dtos.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.UpdateThumbnail;

public class UpdateThumbnailCommand : UpdateThumbnailDto, IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; }

    public void SetId(Guid id)
    {
        Id = id;
    }
}