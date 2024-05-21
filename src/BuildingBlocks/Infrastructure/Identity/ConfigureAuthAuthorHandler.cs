using IdentityServer4.AccessTokenValidation;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;

namespace Infrastructure.Identity;

public static class ConfigureAuthAuthorHandler
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        var configurations = services.GetOptions<ApiConfigurations>(nameof(ApiConfigurations)) ??
                             throw new ArgumentNullException(
                                 $"{nameof(ApiConfigurations)} is not configured properly");

        var issuerUri = configurations.IssuerUri;
        var apiName = configurations.ApiName;

        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(opt =>
            {
                opt.Authority = issuerUri;
                opt.ApiName = apiName;
                opt.RequireHttpsMetadata = false;
                opt.SupportedTokens = SupportedTokens.Both;
            });
    }
    
    public static void AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(IdentityServerAuthenticationDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme); // Specify the policy that will use the "Bearer" authentication scheme (Chỉ định policy sẽ sử dụng scheme xác thực "Bearer")
                    policy.RequireAuthenticatedUser(); // User authentication is required to meet this policy. (Yêu cầu người dùng phải xác thực để đáp ứng policy này.)
                });
            });
    }
}