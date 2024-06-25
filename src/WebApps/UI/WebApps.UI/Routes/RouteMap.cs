namespace WebApps.UI.Routes;

public static class RouteMap
{
    public static void RegisterRoutes(WebApplication app)
    {
        #region Accounts

        app.MapControllerRoute(
            "update-post",
            "/accounts/update-post/{slug}",
            new { controller = "Accounts", action = "UpdatePost" });
        
        app.MapControllerRoute(
            "update-thumbnail",
            "/accounts/update-thumbnail/{id:guid}",
            new { controller = "Accounts", action = "UpdateThumbnail" });

        app.MapControllerRoute(
            "create-new-post",
            "/accounts/create-new-post",
            new { controller = "Accounts", action = "CreatePost" });

        app.MapControllerRoute(
            "profile",
            "/accounts/profile",
            new { controller = "Accounts", action = "Profile" });
        
        app.MapControllerRoute(
            "manage-posts",
            "/accounts/manage-posts",
            new { controller = "Accounts", action = "ManagePosts" });

        #endregion

        #region Posts

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
        
        app.MapControllerRoute(
            "get-comment-by-post-id",
            "/posts/get-comment-by-post-id/{id:guid}",
            new { controller = "Posts", action = "GetCommentsByPostId" });
        
        app.MapControllerRoute(
            "add-new-comment",
            "/posts/add-new-comment",
            new { controller = "Posts", action = "AddNewComment" });

        #endregion

        #region About

        app.MapControllerRoute(
            "about",
            "/about-me",
            new { controller = "About", action = "Index" });

        #endregion
        
        #region Media

        app.MapControllerRoute(
            "media-upload",
            "media/upload",
            new { controller = "Media", action = "UploadImage" });

        #endregion

        #region Default

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        #endregion
    }
}