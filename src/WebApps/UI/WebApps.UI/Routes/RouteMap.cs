
namespace WebApps.UI.Routes;

public static class RouteMap
{
    public static void RegisterRoutes(WebApplication app)
    {
        #region ACCOUNTS

        app.MapControllerRoute(
            "update_post",
            "accounts/update-post/{id:guid}",
            new { controller = "Accounts", action = "UpdatePost" });

        app.MapControllerRoute(
            "update_post_view",
            "accounts/update-post",
            new { controller = "Accounts", action = "UpdatePost" });

        app.MapControllerRoute(
            "update-thumbnail",
            "/accounts/update-thumbnail/{id:guid}",
            new { controller = "Accounts", action = "UpdateThumbnail" });

        app.MapControllerRoute(
            "toggle-pin-status",
            "/accounts/toggle-pin-status/{id:guid}",
            new { controller = "Accounts", action = "TogglePinStatus" });

        app.MapControllerRoute(
            "toggle-featured-status",
            "/accounts/toggle-featured-status/{id:guid}",
            new { controller = "Accounts", action = "ToggleFeaturedStatus" });

        app.MapControllerRoute(
            "delete_post",
            "accounts/delete-post/{id:guid}",
            new { controller = "Accounts", action = "DeletePost" });

        app.MapControllerRoute(
            "approve_post",
            "accounts/approve-post/{id:guid}",
            new { controller = "Accounts", action = "ApprovePost" });

        app.MapControllerRoute(
            "submit_post_for_approval",
            "accounts/waiting-post/{id:guid}",
            new { controller = "Accounts", action = "SubmitPostForApproval" });

        app.MapControllerRoute(
            "reject_post_with_reason",
            "accounts/reject-post/{id:guid}",
            new { controller = "Accounts", action = "RejectPostWithReason" });

        app.MapControllerRoute(
            "posts-by-current-user",
            "/accounts/posts-by-current-user",
            new { controller = "Accounts", action = "GetPostsByCurrentUser" });

        app.MapControllerRoute(
            "manage-posts",
            "/accounts/manage-posts",
            new { controller = "Accounts", action = "ManagePosts" });

        app.MapControllerRoute(
            "create-new-post",
            "/accounts/create-new-post",
            new { controller = "Accounts", action = "CreatePost" });
        
        app.MapControllerRoute(
            "update-profile",
            "/accounts/update-profile/{userId:guid}",
            new { controller = "Accounts", action = "UpdateProfile" });
        
        app.MapControllerRoute(
            "update-avatar",
            "/accounts/update-avatar/{userId:guid}",
            new { controller = "Accounts", action = "UpdateAvatar" });

        app.MapControllerRoute(
            "profile",
            "/accounts/profile",
            new { controller = "Accounts", action = "Profile" });

        #endregion

        #region POSTS

        app.MapControllerRoute(
            "post-detail",
            "post/{slug}",
            new { controller = "Posts", action = "Details" });

        app.MapControllerRoute(
            "posts-by-category",
            "/category/{categorySlug}",
            new { controller = "Posts", action = "PostsByCategory" });

        app.MapControllerRoute(
            "posts-in-series",
            "series/{seriesSlug}",
            new { controller = "Posts", action = "PostsBySeries" });

        app.MapControllerRoute(
            "posts-in-tag",
            "tag/{tagSlug}",
            new { controller = "Posts", action = "PostsByTag" });

        app.MapControllerRoute(
            "posts-by-author",
            "/author/{userName}",
            new { controller = "Posts", action = "PostsByAuthor" });

        app.MapControllerRoute(
            "post-search",
            "/search",
            new { controller = "Posts", action = "Search" });

        #endregion
        
        #region COMMENTS

        app.MapControllerRoute(
            "get-comments-by-post-id",
            "/comments/get-comments-by-post-id",
            new { controller = "Comments", action = "GetCommentsByPostId" });

        app.MapControllerRoute(
            "add-new-comment",
            "/comments/add-new-comment",
            new { controller = "Comments", action = "AddNewComment" });

        app.MapControllerRoute(
            "reply-to-comment",
            "/comments/reply-to-comment",
            new { controller = "Comments", action = "ReplyToComment" });

        #endregion

        #region TAGS

        app.MapControllerRoute(
            "get-suggested-tags",
            "/tags/suggest",
            new { controller = "Tags", action = "GetSuggestedTags" });

        #endregion
        
        #region POST ACTIVITY LOGS

        app.MapControllerRoute(
            "get_post_activity_logs",
            "/posts/activity-logs/{postId:guid}",
            new { controller = "PostActivityLogs", action = "GetPostActivityLogs" });

        #endregion

        #region ABOUT

        app.MapControllerRoute(
            "about",
            "/about-me",
            new { controller = "About", action = "Index" });

        #endregion

        #region DEFAULT

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        #endregion
    }
}