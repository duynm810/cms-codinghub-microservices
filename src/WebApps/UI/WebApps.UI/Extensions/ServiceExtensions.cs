using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Contracts.Commons.Interfaces;
using IdentityModel.Client;
using Infrastructure.Commons;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Settings;
using WebApps.UI.ApiServices;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Attributes;
using WebApps.UI.Services;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register repository services
        services.AddRepositoryAndDomainServices();

        // Register http client services
        services.AddHttpClientServices();

        // Register api client services
        services.AddApiClientServices();

        // Register additional services
        services.AddAdditionalServices();

        // Register razor pages runtime (using for dev)
        services.AddRazorPagesRuntimeConfiguration();

        // Register authentication services
        services.AddAuthenticationServices();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>()
                          ?? throw new ArgumentNullException(
                              $"{nameof(ApiSettings)} is not configured properly");

        services.AddSingleton(apiSettings);

        var identityServerSettings =
            configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>()
            ?? throw new ArgumentNullException(
                $"{nameof(IdentityServerSettings)} is not configured properly");

        services.AddSingleton(identityServerSettings);

        var paginationSettings = configuration.GetSection(nameof(PaginationSettings)).Get<PaginationSettings>()
                                 ?? throw new ArgumentNullException(
                                     $"{nameof(PaginationSettings)} is not configured properly");

        services.AddSingleton(paginationSettings);
    }

    private static void AddApiClientServices(this IServiceCollection services)
    {
        services
            .AddScoped<IBaseApiClient, BaseApiClient>()
            .AddScoped<ICategoryApiClient, CategoryApiClient>()
            .AddScoped<IPostApiClient, PostApiClient>()
            .AddScoped<ISeriesApiClient, SeriesApiClient>()
            .AddScoped<ITagApiClient, TagApiClient>()
            .AddScoped<ICommentApiClient, CommentApiClient>()
            .AddScoped<IMediaApiClient, MediaApiClient>();
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ISerializeService, SerializeService>()
            .AddScoped<IRazorRenderViewService, RazorRenderViewService>()
            .AddScoped<IErrorService, ErrorService>();
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllersWithViews(options => { options.Filters.Add<CustomExceptionFilterAttribute>(); });
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void AddRazorPagesRuntimeConfiguration(this IServiceCollection services)
    {
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (enviroment == Environments.Development)
        {
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }
    }

    private static void AddHttpClientServices(this IServiceCollection services)
    {
        services.AddHttpClient();
    }

    private static void AddAuthenticationServices(this IServiceCollection services)
    {
        var identityServerSettings = services.GetOptions<IdentityServerSettings>(nameof(IdentityServerSettings)) ??
                                     throw new ArgumentNullException(
                                         $"{nameof(IdentityServerSettings)} is not configured properly");

        // Config SSO with Identity Server 4
        IdentityModelEventSource.ShowPII = true;

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async x =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        if (x.Properties.IssuedUtc != null)
                        {
                            var timeElapsed = now.Subtract(x.Properties.IssuedUtc.Value);
                            if (x.Properties.ExpiresUtc != null)
                            {
                                var timeRemaining = x.Properties.ExpiresUtc.Value.Subtract(now);

                                if (timeElapsed > timeRemaining)
                                {
                                    var identity = (ClaimsIdentity)x.Principal?.Identity!;
                                    var accessTokenClaim = identity.FindFirst("access_token");
                                    var refreshTokenClaim = identity.FindFirst("refresh_token");

                                    var refreshToken = refreshTokenClaim?.Value;
                                    if (refreshToken != null)
                                    {
                                        var response = await new HttpClient().RequestRefreshTokenAsync(
                                            new RefreshTokenRequest
                                            {
                                                Address = identityServerSettings.AuthorityUrl,
                                                ClientId = identityServerSettings.ClientId,
                                                ClientSecret = identityServerSettings.ClientSecret,
                                                RefreshToken = refreshToken
                                            });

                                        if (!response.IsError && response is
                                                { AccessToken: not null, RefreshToken: not null })
                                        {
                                            identity.RemoveClaim(accessTokenClaim);
                                            identity.RemoveClaim(refreshTokenClaim);

                                            identity.AddClaims(new[]
                                            {
                                                new Claim("access_token", response.AccessToken),
                                                new Claim("refresh_token", response.RefreshToken)
                                            });

                                            x.ShouldRenew = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = identityServerSettings.AuthorityUrl;
                options.RequireHttpsMetadata = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.MetadataAddress = $"{identityServerSettings.IssuerUri}/.well-known/openid-configuration"; // Fix MVC client connect Identity server with docker
                options.TokenValidationParameters.ValidIssuer = identityServerSettings.AuthorityUrl;
                
                options.ClientId = identityServerSettings.ClientId;
                options.ClientSecret = identityServerSettings.ClientSecret;
                options.ResponseType = "code";
                options.SaveTokens = true;

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.Scope.Add("coding_hub_microservices_api.read");
                options.Scope.Add("coding_hub_microservices_api.write");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        context.ProtocolMessage.IssuerAddress = $"{identityServerSettings.AuthorityUrl}/connect/authorize";
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        context.ProtocolMessage.IssuerAddress = $"{identityServerSettings.AuthorityUrl}/connect/endsession";
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = x =>
                    {
                        var identity = (ClaimsIdentity)x.Principal?.Identity!;
                        var accessToken = x.TokenEndpointResponse?.AccessToken;
                        var refreshToken = x.TokenEndpointResponse?.RefreshToken;

                        if (accessToken == null || refreshToken == null)
                            return Task.CompletedTask;

                        identity.AddClaims(new[]
                        {
                            new Claim("access_token", accessToken),
                            new Claim("refresh_token", refreshToken)
                        });

                        if (x.Properties == null)
                            return Task.CompletedTask;

                        x.Properties.IsPersistent = true;

                        var jwtToken = new JwtSecurityToken(accessToken);
                        x.Properties.ExpiresUtc = jwtToken.ValidTo;

                        return Task.CompletedTask;
                    }
                };
            });
    }
}