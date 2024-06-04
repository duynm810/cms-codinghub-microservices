namespace Shared.Constants;

public static class UserRolesConsts
{
    /// <summary>
    /// User who can subscribe to receive notifications and updates.
    /// </summary>
    public const string Subscriber = "Subscriber ";

    /// <summary>
    /// User who can create and manage their own posts.
    /// </summary>
    public const string Author = "Author";

    /// <summary>
    /// User who can view public content without any editing privileges.
    /// </summary>
    public const string Reader = "Reader";

    /// <summary>
    /// Administrator user with elevated privileges.
    /// </summary>
    public const string Administrator = "Administrator";
}