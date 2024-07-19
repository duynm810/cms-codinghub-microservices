namespace Shared.Helpers;

public static class CacheKeyHelper
{
    #region CATEGORY

    private const string CategoryServicePrefix = "service:category";

    public static class Category
    {
        public static string GetAllCategoriesKey() => $"{CategoryServicePrefix}:all";

        public static string GetCategoryByIdKey(long categoryId) => $"{CategoryServicePrefix}:{categoryId}";

        public static string GetCategoryBySlugKey(string slug) => $"{CategoryServicePrefix}:slug:{slug}";

        public static string GetCategoriesPagingKey(int pageNumber, int pageSize) =>
            $"{CategoryServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion
    
    #region CATEGORY GRPC

    private const string CategoryGrpcPrefix = "grpc:category";

    public static class CategoryGrpc
    {
        public static string GetGrpcAllNonStaticPageCategoriesKey() => $"{CategoryGrpcPrefix}:non_static";
    }
    
    #endregion

    #region POST

    private const string PostServicePrefix = "service:post";

    public static class Post
    {
        public static string GetAllPostsKey() => $"{PostServicePrefix}:all";

        public static string GetPinnedPostsKey() => $"{PostServicePrefix}:pinned";

        public static string GetFeaturedPostsKey() => $"{PostServicePrefix}:featured";

        public static string GetMostLikedPostsKey() => $"{PostServicePrefix}:most_liked";

        public static string GetMostCommentPostsKey() => $"{PostServicePrefix}:most_commented";

        public static string GetPostByIdKey(Guid postId) => $"{PostServicePrefix}:{postId}";
        
        public static string GetPostBySlugKey(string slug) => $"{PostServicePrefix}:slug:{slug}";

        public static string GetPostsByNonStaticPageCategoryKey() => $"{PostServicePrefix}:category:non_static_page";

        public static string GetDetailBySlugKey(string slug) => $"{PostServicePrefix}:detail:slug:{slug}";

        public static string GetPostsByCategoryPagingKey(string categorySlug, int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:category:slug:{categorySlug}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostsBySeriesPagingKey(string seriesSlug, int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:series:slug:{seriesSlug}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostsByAuthorPagingKey(string userName, int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:author:{userName}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region SERIES

    private const string SeriesServicePrefix = "service:series";

    public static class Series
    {
        public static string GetAllSeriesKey() => $"{SeriesServicePrefix}:all";

        public static string GetSeriesByIdKey(Guid seriesId) => $"{SeriesServicePrefix}:{seriesId}";

        public static string GetSeriesBySlugKey(string slug) => $"{SeriesServicePrefix}:slug:{slug}";

        public static string GetSeriesPagingKey(int pageNumber, int pageSize) => $"{SeriesServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region SERIES GRPC

    private const string SeriesGrpcPrefix = "grpc:series";

    public static class SeriesGrpc
    {
        public static string GetAllSeriesKey() => $"{SeriesGrpcPrefix}:all";
    }

    #endregion

    #region POST IN SERIES

    private const string PostInSeriesServicePrefix = "service:post-in-series";

    public static class PostInSeries
    {
        public static string GetAllPostInSeriesByIdKey(Guid seriesId) => $"{PostInSeriesServicePrefix}:all:{seriesId}";
        
        public static string GetPostInSeriesBySlugKey(string slug) => $"{PostInSeriesServicePrefix}:slug:{slug}";
        
        public static string GetPostInSeriesByIdPagingKey(Guid seriesId, int pageNumber, int pageSize) =>
            $"{PostInSeriesServicePrefix}:series:id:{seriesId}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostInSeriesBySlugPagingKey(string seriesSlug, int pageNumber, int pageSize) =>
            $"{PostInSeriesServicePrefix}:series:slug:{seriesSlug}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region TAG

    private const string TagServicePrefix = "service:tag";
    
    public static class Tag
    {
        public static string GetTagByIdKey(Guid tagId) => $"{TagServicePrefix}:{tagId}";
        
        public static string GetTagBySlugKey(string slug) => $"{TagServicePrefix}:slug:{slug}";
        
        public static string GetTagByNameKey(string name) => $"{TagServicePrefix}:name:{name}";

        public static string GetTagsPagingKey(int pageNumber, int pageSize) => $"{TagServicePrefix}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetSuggestedTagsKey(string? keyword, int count) => $"{TagServicePrefix}:suggested:keyword:{keyword}:count:{count}";
    }

    #endregion
    
    #region POST IN TAG

    private const string PostInTagServicePrefix = "service:post-in-tag";

    public static class PostInTag
    {
        public static string GetAllPostInTagByIdKey(Guid tagId) => $"{PostInTagServicePrefix}:all:{tagId}";
        
        public static string GetPostInTagBySlugKey(string slug) => $"{PostInTagServicePrefix}:slug:{slug}";
        
        public static string GetPostInTagByIdPagingKey(Guid tagId, int pageNumber, int pageSize) =>
            $"{PostInTagServicePrefix}:tag:id:{tagId}:page:{pageNumber}:size:{pageSize}";
        
        public static string GetPostInTagBySlugPagingKey(string tagSlug, int pageNumber, int pageSize) =>
            $"{PostInTagServicePrefix}:tag:slug:{tagSlug}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion
}