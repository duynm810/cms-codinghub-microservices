namespace Shared.Constants;

public static class ErrorMessagesConsts
{
    public static class Common
    {
        public const string UnhandledException = "Unhandled exception";
    }

    public static class Cache
    {
        public const string CacheKeyRemovalError = "An error occurred while removing cache keys.";
    }

    public static class Authentication
    {
        public const string InvalidCredentials = "Invalid credentials provided.";
        public const string TokenExpired = "Authentication token has expired.";
    }

    public static class Network
    {
        public const string ServerUrlNotConfigured = "Server URL must be configured.";
        public const string RequestFailed = "Request failed with status code {0} and content {1}";
    }

    public static class Data
    {
        public const string DeserializeFailed = "Failed to deserialize the response content.";
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
        public const string InvalidGetPostsByNonStaticPageCategoryNotFound = "Invalid get post by non static page category";
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
    
    public static class Tag
    {
        public const string TagNotFound = "Tag not found";
        public const string InvalidTagId = "Invalid tag ID";
    }
    
    public static class PostInTag
    {
        public const string PostIdsNotFound = "Post Ids not found";
        public const string PostNotFoundInTag = "No posts found in this tag";
    }

    public static class Comment
    {
        public const string ParentIdNotFound = "Parent Id not found";
        public const string CommentCreationFailed = "Failed to create comment";
        public const string RepliesCountUpdateFailed = "Failed to update replies count";
    }

    public static class Identity
    {
        public static class Permission
        {
            public const string PermissionCreationFailed = "Failed to create permission.";
        }

        public static class Role
        {
            public const string RoleNotFound = "Role not found";
            public const string RoleCreationFailed = "Failed to create role";
            public const string RoleUpdateFailed = "Failed to update role";
            public const string RoleDeleteFailed = "Failed to delete role";
        }

        public static class User
        {
            public const string UserNotFound = "User not found";
            public const string UserNotAuthenticated = "User not authenticated";
            public const string UserCreationFailed = "Failed to create user";
            public const string UserUpdateFailed = "Failed to update user";
            public const string UserUpdateAvatarFailed = "Failed to update avatar for user";
            public const string UserDeleteFailed = "Failed to delete user";
            public const string UserChangePasswordFailed = "Failed to change password user";
        }
    }

    public static class Media
    {
        public const string FileNotUploaded = "No files have been uploaded";
        public const string FileIsEmpty = "The file is empty";
        public const string InvalidFileTypeOrName = "Invalid file type or name";
        public const string FileNameCannotBeEmpty = "Filename cannot be null or empty";
        public const string ImageUploadFailed = "Image upload failed with status code: {0}, error: {1}";
    }
}