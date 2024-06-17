using AutoMapper;
using Post.Application.Commons.Mappings;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Tag;
using Shared.Enums;

namespace Post.Application.Commons.Models;

public class PostModel : IMapFrom<PostBase>, IMapFrom<CategoryDto>
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? Content { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public int ViewCount { get; set; }

    public int CommentCount { get; set; }

    public int LikeCount { get; set; }

    public bool IsPinned { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsPaid { get; set; }

    public double RoyaltyAmount { get; set; }

    public PostStatusEnum Status { get; set; }

    public DateTimeOffset? PublishedDate { get; set; }

    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public string? CategorySeoDescription { get; set; }

    public string? CategoryIcon { get; set; }

    public string? CategoryColor { get; set; }

    public DateTimeOffset? PaidDate { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
    
    /// <summary>
    /// List of tags in the article (Danh sách các thẻ thuộc bài viết)
    /// </summary>
    public List<TagDto>? Tags { get; set; }
    
    /// <summary>
    /// Article author information (Thông tin tác giả bài viết)
    /// </summary>
    public UserDto? User { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PostBase, PostModel>();

        // Ignore the Id, Slug, and SeoDescription mapping because it will confuse duplicate fields with each other
        // Bỏ qua ánh xạ Id, Slug, SeoDescription vì sẽ nhầm lẫn trùng field với các bảng với nhau
        profile.CreateMap<CategoryDto, PostModel>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.Slug,
                opt => opt.Ignore())
            .ForMember(dest => dest.SeoDescription,
                opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategorySlug,
                opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.CategorySeoDescription,
                opt => opt.MapFrom(src => src.SeoDescription))
            .ForMember(dest => dest.CategoryIcon,
                opt => opt.MapFrom(src => src.Icon))
            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Color));
    }
}