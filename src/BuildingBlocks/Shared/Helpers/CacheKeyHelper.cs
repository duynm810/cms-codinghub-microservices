namespace Shared.Helpers;

public static class CacheKeyHelper
{
    #region Category
    
    private const string CategoryGrpcPrefix = "grpc:category";
    
    private const string CategoryServicePrefix = "service:category";

    public static class Category
    {
        public static string GetAllCategoriesKey() => $"{CategoryServicePrefix}:all";

        public static string GetCategoryByIdKey(long categoryId) => $"{CategoryServicePrefix}:{categoryId}";

        public static string GetCategoryBySlugKey(string slug) => $"{CategoryServicePrefix}:slug:{slug}";

        public static string GetCategoriesPagingKey(int pageNumber, int pageSize) => $"{CategoryServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    public static class CategoryGrpc
    {
        public static string GetGrpcCategoryByIdKey(long categoryId) => $"{CategoryGrpcPrefix}:{categoryId}";
        
        public static string GetGrpcCategoryBySlugKey(string slug) => $"{CategoryGrpcPrefix}:slug:{slug}";
        
        public static string GetGrpcCategoriesByIdsKey(IEnumerable<long> ids) => $"{CategoryGrpcPrefix}:ids:{string.Join("_", ids)}";
        
        public static string GetGrpcAllNonStaticPageCategoriesKey() => $"{CategoryGrpcPrefix}:non_static";
    }

    #endregion

    #region Post

    public static class Post
    {
        public static string GetAllPostsKey() => "posts:all";

        public static string GetPinnedPostsKey() => "posts:pinned";

        public static string GetFeaturedPostsKey() => "posts:featured";

        public static string GetPostByIdKey(Guid postId) => $"post:{postId}";

        public static string GetPostsByCategoryPagingKey(string categorySlug, int pageNumber, int pageSize) =>
            $"posts:category:{categorySlug}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion
}