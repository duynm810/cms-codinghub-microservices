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

        #endregion

        #region Posts

        app.MapControllerRoute(
            "posts-by-category",
            "/category/{categorySlug}",
            new { controller = "Posts", action = "PostsByCategory" });
        
        app.MapControllerRoute(
            "posts-by-tag",
            "/tag/{tagSlug}",
            new { controller = "Posts", action = "PostsByTag" });

        app.MapControllerRoute(
            "post-detail",
            "post/{slug}",
            new { controller = "Posts", action = "Details" });

        app.MapControllerRoute(
            "post-search",
            "/search",
            new { controller = "Posts", action = "Search" });

        #endregion

        #region Series

        app.MapControllerRoute(
            "posts-in-series",
            "series/{slug}",
            new { controller = "Posts", action = "PostsInSeries" });

        #endregion
    }
}