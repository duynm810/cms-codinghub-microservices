using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetDetailBySlug;

public class GetDetailBySlugQuery(string slug, int relatedCount) : IRequest<ApiResult<PostsBySlugDto>>
{
    public string Slug { get; set; } = slug;

    public int RelatedCount { get; set; } = relatedCount;
}