namespace Shared.Helpers;

public static class CacheKeyHelper
{
    #region Category

    public static class Category
    {
        public static string GetAllCategoriesKey() => "categories:all";

        public static string GetCategoryByIdKey(long categoryId) => $"category:{categoryId}";

        public static string GetCategoryBySlugKey(string slug) => $"category:slug:{slug}";

        public static string GetCategoriesPagingKey(int pageNumber, int pageSize) => $"categories:page:{pageNumber}:size:{pageSize}";
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