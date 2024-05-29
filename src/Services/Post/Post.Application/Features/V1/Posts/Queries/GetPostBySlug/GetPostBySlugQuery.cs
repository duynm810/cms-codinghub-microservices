using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQuery(string slug) : IRequest<ApiResult<PostDto>>
{
    public string Slug { get; set; } = slug;
}