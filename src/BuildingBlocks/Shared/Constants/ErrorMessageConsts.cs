namespace Shared.Constants;

public static class ErrorMessageConsts
{
    public static class Common
    {
        public const string UnhandledException = "Unhandled exception";
    }
    
    public static class Category
    {
        public const string CategoryNotFound = "Category not found";
        public const string CategoryContainsPost = "The category contains posts, cannot be deleted!";
        public const string InvalidCategoryId = "Invalid category ID";
    }

    public static class Post
    {
        public const string PostNotFound = "Post not found";
        public const string SlugExists = "Slug is exists";
    }

    public static class Series
    {
        public const string SeriesNotFound = "Series not found";
    }

    public static class PostInSeries
    {
        public const string PostIdsNotFound = "Post Ids not found";
        public const string PostNotFoundInSeries = "No posts found in this series";
    }
}