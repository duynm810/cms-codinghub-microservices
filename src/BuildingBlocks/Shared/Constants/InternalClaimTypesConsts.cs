namespace Shared.Constants;

public static class InternalClaimTypesConsts
{
    public const string IdentitySchema = "Identity";

    public static class Claims
    {
        public const string Roles = "roles";
        public const string Permissions = "permissions";
        public const string UserId = "id";
        public const string UserName = "userName";
        public const string FirstName = "firstName";
        public const string LastName = "lastName";
        public const string FullName = "fullName";
        public const string DeviceId = "deviceId";
        public const string Device = "device";
        public const string Client = "client";
        public const string Version = "version";
        public const string Token = "token";
        public const string IsApiKey = "isApiKey";
    }
}