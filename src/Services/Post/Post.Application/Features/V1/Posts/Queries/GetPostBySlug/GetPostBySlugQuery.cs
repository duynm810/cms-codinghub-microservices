using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQuery(string slug, int relatedCount) : IRequest<ApiResult<PostBySlugDto>>
{
    public string Slug { get; set; } = slug;

    public int RelatedCount { get; set; } = relatedCount;
}