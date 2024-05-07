using AutoMapper;
using Post.Application.Commons.Mappings;
using Post.Domain.Entities;
using Shared.Enums;

namespace Post.Application.Features.V1.Posts.Commons;

public class CreateOrUpdateCommand : IMapFrom<PostBase>
{
    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public string? Tags { get; set; }

    public int ViewCount { get; set; }

    public bool IsPaid { get; set; }

    public double RoyaltyAmount { get; set; }

    public PostStatusEnum Status { get; set; }

    public Guid CategoryId { get; set; }

    public Guid AuthorUserId { get; set; }

    public DateTime? PaidDate { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrUpdateCommand, PostBase>();
    }
}