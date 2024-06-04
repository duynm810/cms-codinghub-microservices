using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Infrastructure.Persistence;
using Shared.Enums;
using Shared.Responses;

namespace Post.Infrastructure.Repositories;

public class PostRepository(PostContext dbContext, IUnitOfWork<PostContext> unitOfWork)
    : RepositoryCommandBase<PostBase, Guid, PostContext>(dbContext, unitOfWork), IPostRepository
{
    #region CRUD

    public Guid CreatePost(PostBase post)
    {
        Create(post);
        return post.Id;
    }

    public async Task<PostBase> UpdatePost(PostBase post)
    {
        await UpdateAsync(post);
        return post;
    }

    public async Task DeletePost(PostBase post) => await DeleteAsync(post);

    public async Task<IEnumerable<PostBase>> GetPosts() => await FindAll().ToListAsync();

    public async Task<PostBase?> GetPostById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<PagedResponse<PostBase>> GetPostsPaging(string? filter, int pageNumber, int pageSize)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published);

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => (x.Title.Contains(filter))
                                     || (x.Slug.Contains(filter))
                                     || (x.Content != null && x.Content.Contains(filter))
                                     || (x.Summary != null && x.Summary.Contains(filter))
                                     || (x.Tags != null && x.Tags.Contains(filter)));
        }

        query = query.OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, pageNumber, pageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetPostsByCategoryPaging(long categoryId, int pageNumber = 1,
        int pageSize = 10)
    {
        var query = FindByCondition(x => x.CategoryId == categoryId && x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, pageNumber, pageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetLatestPostsPaging(int pageNumber, int pageSize)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, pageNumber, pageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<IEnumerable<PostBase>> GetPostsByCategoryId(long categoryId, int count) =>
        await FindByCondition(x => x.CategoryId == categoryId && x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate).Take(count).ToListAsync();

    public async Task<PostBase?> GetPostBySlug(string slug) =>
        await FindByCondition(x => x.Slug == slug).FirstOrDefaultAsync() ?? null;

    public async Task<IEnumerable<PostBase>> GetPostsByIds(Guid[] ids)
    {
        return await FindByCondition(c => ids.Contains(c.Id)).ToListAsync();
    }

    public async Task<IEnumerable<PostBase>> GetRelatedPosts(PostBase post, int count)
    {
        var relatedPostsQuery = FindByCondition(x => x.Id != post.Id
                                                     && x.Status == PostStatusEnum.Published);

        // Take double the count to ensure sufficient posts from both criteria
        // Tăng số lượng kết quả tạm thời lên gấp đôi (count * 2) đảm bảo đủ bài viết từ cả hai tiêu chí, thậm chí sau khi loại bỏ trùng lặp.
        var relatedPosts = await relatedPostsQuery
            .Where(x => x.CategoryId == post.CategoryId || x.AuthorUserId == post.AuthorUserId)
            .OrderByDescending(x => x.ViewCount)
            .Take(count * 2)
            .ToListAsync();

        var finalRelatedPosts = relatedPosts
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .OrderByDescending(x => x.ViewCount)
            .Take(count)
            .ToList();

        return finalRelatedPosts;
    }

    public async Task<IEnumerable<PostBase>> GetFeaturedPosts(int count)
    {
        // Get featured articles first (Lấy các bài viết nổi bật trước)
        var featuredPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && x.IsFeatured)
            .OrderByDescending(x => x.ViewCount)
            .Take(count)
            .ToListAsync();

        // If the number of featured posts is less than required, add non-featured posts (Nếu số lượng bài viết nổi bật ít hơn yêu cầu, bổ sung thêm các bài viết không nổi bật)
        if (featuredPosts.Count < count)
        {
            var additionalPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && !x.IsFeatured)
                .OrderByDescending(x => x.ViewCount)
                .Take(count - featuredPosts.Count)
                .ToListAsync();

            featuredPosts.AddRange(additionalPosts);
        }

        return featuredPosts;
    }

    public async Task<IEnumerable<PostBase>> GetPinnedPosts(int count)
    {
        // Get pinned posts first (Lấy các bài viết ghim trước)
        var pinnedPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && x.IsPinned)
            .OrderByDescending(x => x.ViewCount)
            .Take(count)
            .ToListAsync();

        // If the number of pinned posts is less than required, add non-pinned posts (Nếu số lượng bài viết ghim ít hơn yêu cầu, bổ sung thêm các bài viết không ghim)
        if (pinnedPosts.Count < count)
        {
            var additionalPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && !x.IsPinned)
                .OrderByDescending(x => x.ViewCount)
                .Take(count - pinnedPosts.Count)
                .ToListAsync();

            pinnedPosts.AddRange(additionalPosts);
        }

        return pinnedPosts;
    }

    public async Task<IEnumerable<PostBase>> GetMostCommentPosts(int count)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.CommentCount)
            .Take(count);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<PostBase>> GetMostLikedPosts(int count)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.LikeCount)
            .Take(count);

        return await query.ToListAsync();
    }

    public async Task<bool> SlugExists(string slug, Guid? currentId = null)
    {
        return await FindByCondition(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value))
            .AnyAsync();
    }

    public async Task<bool> HasPostsInCategory(long categoryId)
    {
        return await FindByCondition(x => x.CategoryId == categoryId).AnyAsync();
    }

    public async Task ApprovePost(PostBase post)
    {
        post.Status = PostStatusEnum.Published;
        post.PublishedDate = DateTime.UtcNow;
        await UpdateAsync(post);
    }

    public async Task SubmitPostForApproval(PostBase post)
    {
        post.Status = PostStatusEnum.WaitingForApproval;
        await UpdateAsync(post);
    }

    public async Task RejectPostWithReason(PostBase post)
    {
        post.Status = PostStatusEnum.Rejected;
        await UpdateAsync(post);
    }

    #endregion
}