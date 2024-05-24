namespace Shared.Constants;

public static class ErrorMessagesConsts
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

    public static class Identity
    {
        public static class Permission
        {
            public const string PermissionCreationFailed = "Failed to create permission.";
        }

        public static class Role
        {
            public const string RoleCreationFailed = "Failed to create role";
            public const string RoleUpdateFailed = "Failed to update role";
            public const string RoleDeleteFailed = "Failed to delete role";
        }
    }
}