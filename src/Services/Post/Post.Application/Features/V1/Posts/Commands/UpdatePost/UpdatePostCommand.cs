using AutoMapper;
using MediatR;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Features.V1.Posts.Commons;
using Post.Domain.Entities;
using Shared.Dtos.Post.Commands;
using Shared.Dtos.Post.Queries;
using Shared.Extensions;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.UpdatePost;

public class UpdatePostCommand : CreateOrUpdateCommand, IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; }
    
    public Guid AuthorUserId { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }
}