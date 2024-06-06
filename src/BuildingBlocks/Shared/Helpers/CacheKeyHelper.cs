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

        public static string GetCategoriesPagingKey(int pageNumber, int pageSize) =>
            $"{CategoryServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region Category Grpc

    private const string CategoryGrpcPrefix = "grpc:category";

    public static class CategoryGrpc
    {
        public static string GetGrpcCategoryByIdKey(long categoryId) => $"{CategoryGrpcPrefix}:{categoryId}";

        public static string GetGrpcCategoryBySlugKey(string slug) => $"{CategoryGrpcPrefix}:slug:{slug}";

        public static string GetGrpcCategoriesByIdsKey(IEnumerable<long> ids) =>
            $"{CategoryGrpcPrefix}:ids:{string.Join("_", ids)}";

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
            $"{PostServicePrefix}:category:slug:{categorySlug}:page:{pageNumber}:size:{pageSize}";

        public static string GetPostsPagingKey(int pageNumber, int pageSize) =>
            $"{PostServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region Post Grpc

    private const string PostGrpcPrefix = "grpc:post";

    public static class PostGrpc
    {
        public static string GetGrpcPostsByIdsKey(IEnumerable<Guid> ids) =>
            $"{PostGrpcPrefix}:ids:{string.Join("_", ids)}";
    }

    #endregion

    #region Series

    private const string SeriesServicePrefix = "service:series";

    public static class Series
    {
        public static string GetAllSeriesKey() => $"{SeriesServicePrefix}:all";

        public static string GetSeriesByIdKey(Guid seriesId) => $"{SeriesServicePrefix}:{seriesId}";

        public static string GetSeriesBySlugKey(string slug) => $"{SeriesServicePrefix}:slug:{slug}";

        public static string GetSeriesPagingKey(int pageNumber, int pageSize) =>
            $"{SeriesServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region Series Grpc

    private const string SeriesGrpcPrefix = "grpc:series";

    public static class SeriesGrpc
    {
        public static string GetGrpcSeriesByIdKey(Guid seriesId) => $"{SeriesGrpcPrefix}:{seriesId}";

        public static string GetGrpcSeriesBySlugKey(string slug) => $"{SeriesGrpcPrefix}:slug:{slug}";
    }

    #endregion

    #region Post In Series

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

    #region Tag

    private const string TagServicePrefix = "service:tag";
    
    public static class Tag
    {
        public static string GetAllTagsKey() => $"{TagServicePrefix}:all";

        public static string GetTagByIdKey(Guid tagId) => $"{TagServicePrefix}:{tagId}";

        public static string GetTagsPagingKey(int pageNumber, int pageSize) =>
            $"{TagServicePrefix}:page:{pageNumber}:size:{pageSize}";
    }

    #endregion

    #region Tag Grpc

    private const string TagGrpcPrefix = "grpc:tag";

    public static class TagGrpc
    {
        public static string GetAllTagsKey() => $"{TagGrpcPrefix}:all";
        
        public static string GetGrpcTagsByIdsKey(IEnumerable<Guid> ids) =>
            $"{TagGrpcPrefix}:ids:{string.Join(",", ids)}";
    }

    #endregion
}