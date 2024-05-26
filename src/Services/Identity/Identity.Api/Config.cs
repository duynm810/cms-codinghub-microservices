using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.Api;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new()
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("coding_hub_microservices_api.read", "Coding Hub Microservices Api Read Scope"),
            new("coding_hub_microservices_api.write", "Coding Hub Microservices Api Write Scope")
        };

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
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:5002/swagger/oauth2-redirect.html",
                    "http://localhost:5003/swagger/oauth2-redirect.html",
                    "http://localhost:5005/swagger/oauth2-redirect.html",
                    "http://localhost:5007/swagger/oauth2-redirect.html",
                    "http://localhost:5009/swagger/oauth2-redirect.html",
                    "http://localhost:6001/swagger/oauth2-redirect.html",
                    "http://localhost:6002/swagger/oauth2-redirect.html",
                    "http://localhost:6003/swagger/oauth2-redirect.html",
                    "http://localhost:6005/swagger/oauth2-redirect.html",
                    "http://localhost:6007/swagger/oauth2-redirect.html",
                    "http://localhost:6009/swagger/oauth2-redirect.html",
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:5002/swagger/oauth2-redirect.html",
                    "http://localhost:5003/swagger/oauth2-redirect.html",
                    "http://localhost:5005/swagger/oauth2-redirect.html",
                    "http://localhost:5007/swagger/oauth2-redirect.html",
                    "http://localhost:5009/swagger/oauth2-redirect.html",
                    "http://localhost:6001/swagger/oauth2-redirect.html",
                    "http://localhost:6002/swagger/oauth2-redirect.html",
                    "http://localhost:6003/swagger/oauth2-redirect.html",
                    "http://localhost:6005/swagger/oauth2-redirect.html",
                    "http://localhost:6007/swagger/oauth2-redirect.html",
                    "http://localhost:6009/swagger/oauth2-redirect.html",
                },
                AllowedCorsOrigins = new List<string>()
                {
                    "http://localhost:5001",
                    "http://localhost:5002",
                    "http://localhost:5003",
                    "http://localhost:5005",
                    "http://localhost:5007",
                    "http://localhost:5009",
                    "http://localhost:6001",
                    "http://localhost:6002",
                    "http://localhost:6003",
                    "http://localhost:6005",
                    "http://localhost:6007",
                    "http://localhost:6009",
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
                ClientSecrets = new[]
                {
                    new Secret("mvc-client-secret")
                },
                RedirectUris = new List<string>
                {
                    "http://localhost:4000/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:4000/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "coding_hub_microservices_api.read",
                    "coding_hub_microservices_api.write",
                    "roles"
                },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}