namespace Shared.Helpers;

public static class CacheKeyHelper
{
    #region Category
    
    private const string CategoryServicePrefix = "service:category";

    public static class Category
    {
        public static string GetAllCategoriesKey() => $"{CategoryServicePrefix}:all";

        public static string GetCategoryByIdKey(long categoryId) => $"{CategoryServicePrefix}:{categoryId}";

        public static string GetCategoryBySlugKey(string slug) => $"{CategoryServicePrefix}:slug:{slug}";

        public static string GetCategoriesPagingKey(int pageNumber, int pageSize) => $"{CategoryServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region Category Grpc

    private const string CategoryGrpcPrefix = "grpc:category";

    public static class CategoryGrpc
    {
        public static string GetGrpcCategoryByIdKey(long categoryId) => $"{CategoryGrpcPrefix}:{categoryId}";
        
        public static string GetGrpcCategoryBySlugKey(string slug) => $"{CategoryGrpcPrefix}:slug:{slug}";
        
        public static string GetGrpcCategoriesByIdsKey(IEnumerable<long> ids) => $"{CategoryGrpcPrefix}:ids:{string.Join("_", ids)}";
        
        public static string GetGrpcAllNonStaticPageCategoriesKey() => $"{CategoryGrpcPrefix}:non_static";
    }

    #endregion

    #region Post

    private const string PostServicePrefix = "service:post";
    
    public static class Post
    {
        public static string GetAllPostsKey() => $"{PostServicePrefix}:all";

        public static string GetPinnedPostsKey() => $"{PostServicePrefix}:pinned";

        public static string GetFeaturedPostsKey() => $"{PostServicePrefix}:featured";
        
        public static string GetMostLikedPostsKey() => $"{PostServicePrefix}:most_liked";
        
        public static string GetMostCommentPostsKey() => $"{PostServicePrefix}:most_commented";

        public static string GetPostByIdKey(Guid postId) => $"{PostServicePrefix}:{postId}";
        
        public static string GetPostsByNonStaticPageCategoryKey() => $"{PostServicePrefix}:category:non_static_page";

        public static string GetPostBySlugKey(string slug) => $"{PostServicePrefix}:slug:{slug}";

        public static string GetLatestPostsPagingKey(int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:latest:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostsByCategoryPagingKey(string categorySlug, int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:category:{categorySlug}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostsPagingKey(int pageNumber, int pageSize) => $"{PostServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion
}