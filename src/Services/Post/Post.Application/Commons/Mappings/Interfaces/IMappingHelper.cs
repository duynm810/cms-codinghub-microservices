using Post.Application.Commons.Models;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Responses;

namespace Post.Application.Commons.Mappings.Interfaces;

public interface IMappingHelper
{
    List<PostModel> MapPostsWithCategories(IEnumerable<PostBase>? postBases, IEnumerable<CategoryDto> categories);

    PagedResponse<PostModel> MapPostsWithCategories(PagedResponse<PostBase> pagedPosts,
        IEnumerable<CategoryDto> categories);

    PagedResponse<PostModel> MapPostsWithCategory(PagedResponse<PostBase> pagedPosts, CategoryDto category);

    PostModel MapPostWithCategory(PostBase post, CategoryDto category);
}