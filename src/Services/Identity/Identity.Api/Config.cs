using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.Api;

public static class Config
{
    /// <summary>
    /// Defines essential user information (roles, openid, profile, email) which are necessary for authenticating and identifying users in the application.
    /// Định nghĩa thông tin người dùng thiết yếu (roles, openid, profile, email) cần thiết cho việc xác thực và nhận diện người dùng trong ứng dụng.
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new()
            {
                Name = "roles",
                DisplayName = "User role(s)",
                UserClaims = new List<string> { "roles" }
            }
        };

    /// <summary>
    /// Specifies the access level to the API resources. There are two defined scopes for reading and writing within the "Coding Hub Microservices API", allowing fine-grained access control over API operations.
    /// Xác định mức độ truy cập đến các tài nguyên API. Có hai scope được định nghĩa để đọc và viết trong "Coding Hub Microservices API", cho phép kiểm soát truy cập chi tiết đến các thao tác API.
    /// </summary>
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("coding_hub_microservices_api.read", "Coding Hub Microservices Api Read Scope"),
            new("coding_hub_microservices_api.write", "Coding Hub Microservices Api Write Scope")
        };

    /// <summary>
    /// Represents the API that is secured by Identity Server. It includes the scopes that are required to access this API and the user claims that should be included in the access token, specifically the roles claim for authorization purposes.
    /// Đại diện cho API được bảo mật bởi Identity Server. Nó bao gồm các scopes cần thiết để truy cập API này và các claims của người dùng nên được bao gồm trong token truy cập, cụ thể là claim roles cho mục đích ủy quyền.
    /// </summary>
    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("coding_hub_microservices_api", "Coding Hub Microservices Api")
            {
                Scopes = new List<string> { "coding_hub_microservices_api.read", "coding_hub_microservices_api.write" },
                UserClaims = new List<string> { "roles" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientName = "Coding Hub Microservices Swagger Client",
                ClientId = "coding_hub_microservices_swagger",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                RedirectUris = new List<string>()
                {
                    
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    
                },
                AllowedCorsOrigins = new List<string>()
                {
                    
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "coding_hub_microservices_api.read",
                    "coding_hub_microservices_api.write",
                    "roles"
                }
            },
            new()
            {
                ClientName = "Coding Hub Microservices Postman Client",
                ClientId = "coding_hub_microservices_postman",
                Enabled = true,
                ClientUri = null,
                RequireClientSecret = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                AllowOfflineAccess = true,
                ClientSecrets = new[]
                {
                    new Secret("SuperStrongSecret".Sha512())
                },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword
                },
                RedirectUris = new List<string>
                {
                    "https://www.getpostman.com/oauth2/callback"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "coding_hub_microservices_api.read",
                    "coding_hub_microservices_api.write",
                    "roles",
                }
            },
            new()
            {
                ClientId = "coding_hub_microservices_mvc",
                ClientName = "Coding Hub Microservices MVC Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientSecrets = new[]
                {
                    new Secret("mvc-client-secret".Sha512())
                },
                RedirectUris = new List<string>
                {
                    "http://localhost:5300/signin-oidc",
                    "http://localhost:6300/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:5300/signout-callback-oidc",
                    "http://localhost:6300/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "coding_hub_microservices_api.read",
                    "coding_hub_microservices_api.write"
                }
            }
        };
}