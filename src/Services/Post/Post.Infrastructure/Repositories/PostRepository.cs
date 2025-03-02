using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Infrastructure.Persistence;
using Shared.Constants;
using Shared.Enums;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
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

    public async Task<bool> UpdatePost(PostBase post)
    {
        await UpdateAsync(post);
        return true;
    }

    public async Task DeletePost(PostBase post) => await DeleteAsync(post);

    public async Task<IEnumerable<PostBase>> GetPosts() => await FindAll().ToListAsync();

    public async Task<PostBase?> GetPostById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<PagedResponse<PostBase>> GetPostsPaging(GetPostsRequest request)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published);

        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x => (x.Title.Contains(request.Filter))
                                     || (x.Slug.Contains(request.Filter))
                                     || (x.Content != null && x.Content.Contains(request.Filter))
                                     || (x.Summary != null && x.Summary.Contains(request.Filter)));
        }

        query = query.OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, request.PageNumber, request.PageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetPostsByCategoryPaging(long categoryId, GetPostsByCategoryRequest request)
    {
        var query = FindByCondition(x => x.CategoryId == categoryId && x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, request.PageNumber, request.PageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetPostsByAuthorPaging(Guid authorId, GetPostsByAuthorRequest request)
    {
        var query = FindByCondition(x => x.AuthorUserId == authorId && x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, request.PageNumber, request.PageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetPostsByCurrentUserPaging(Guid userId, List<string> roles, GetPostsByCurrentUserRequest request)
    {
        IQueryable<PostBase> query;
        
        var isAdmin = roles.Contains(UserRolesConsts.Administrator);
        
        if (isAdmin)
        {
            query = FindAll();
            
            if (request.Status.HasValue)
            {
                var statusEnum = (PostStatusEnum)request.Status.Value;
                query = query.Where(x => x.Status == statusEnum);
            }
            
            if (request.UserId.HasValue)
            {
                query = query.Where(x => x.AuthorUserId == request.UserId.Value);
            }
            
            query = query.OrderByDescending(x => x.CreatedDate);
        }
        else
        {
            query = FindByCondition(x => x.AuthorUserId == userId)
                .OrderByDescending(x => x.CreatedDate);
        }
        
        var items = await PagedList<PostBase>.ToPagedList(query, request.PageNumber, request.PageSize);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<PagedResponse<PostBase>> GetLatestPostsPaging(GetLatestPostsRequest request)
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.PublishedDate);

        var items = await PagedList<PostBase>.ToPagedList(query, request.PageNumber, request.PageSize);

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

        if (relatedPosts.Count == 0)
        {
            return new List<PostBase>();
        }

        var finalRelatedPosts = relatedPosts
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .OrderByDescending(x => x.ViewCount)
            .Take(count)
            .ToList();

        return finalRelatedPosts;
    }

    public async Task<IEnumerable<PostBase>> GetFeaturedPosts()
    {
        var featuredPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && x.IsFeatured)
            .OrderByDescending(x => x.ViewCount)
            .ToListAsync();

        return featuredPosts;
    }

    public async Task<IEnumerable<PostBase>> GetPinnedPosts()
    {
        var pinnedPosts = await FindByCondition(x => x.Status == PostStatusEnum.Published && x.IsPinned)
            .OrderByDescending(x => x.ViewCount)
            .ToListAsync();

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

    public async Task<IEnumerable<PostBase>> GetTop10Posts()
    {
        var query = FindByCondition(x => x.Status == PostStatusEnum.Published)
            .OrderByDescending(x => x.CreatedDate)
            .Take(10);

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

    public async Task<bool> TogglePinStatus(PostBase post, bool isPinned)
    {
        post.IsPinned = isPinned;
        await UpdateAsync(post);
        return true;
    }

    public async Task<bool> ToggleFeaturedStatus(PostBase post, bool isFeatured)
    {
        post.IsFeatured = isFeatured;
        await UpdateAsync(post);
        return true;
    }

    #endregion
}