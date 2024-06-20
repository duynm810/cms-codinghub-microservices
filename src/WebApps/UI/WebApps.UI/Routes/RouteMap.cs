namespace WebApps.UI.Routes;

public static class RouteMap
{
    public static void RegisterRoutes(WebApplication app)
    {
        #region Default

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        #endregion

        #region About

        app.MapControllerRoute(
            "about",
            "/about-me",
            new { controller = "About", action = "Index" });

        #endregion

        #region Accounts

        app.MapControllerRoute(
            "profile",
            "/accounts/profile",
            new { controller = "Accounts", action = "Profile" });
        
        app.MapControllerRoute(
            "manage-posts",
            "/accounts/manage-posts",
            new { controller = "Accounts", action = "ManagePosts" });
        
        app.MapControllerRoute(
            "create-new-post",
            "/accounts/create-new-post",
            new { controller = "Accounts", action = "CreatePost" });

        #endregion

        #region Posts

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
            "post-detail",
            "post/{slug}",
            new { controller = "Posts", action = "Details" });

        app.MapControllerRoute(
            "post-search",
            "/search",
            new { controller = "Posts", action = "Search" });

        #endregion
    }
}