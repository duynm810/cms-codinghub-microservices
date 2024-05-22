using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Shared.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        var apiConfigurations = services.GetOptions<ApiConfigurations>(nameof(ApiConfigurations)) ??
                                throw new ArgumentNullException(
                                    $"{nameof(ApiConfigurations)} is not configured properly");
        
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc(apiConfigurations.ApiVersion, new OpenApiInfo
            {
                Title = apiConfigurations.ApiTitle,
                Version = apiConfigurations.ApiVersion
            });
            
            c.AddSecurityDefinition(IdentityServerAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{apiConfigurations.IdentityBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "coding_hub_microservices_api.read", "Coding Hub Microservices API Read Scope" },
                            { "coding_hub_microservices_api.write", "Coding Hub Microservices API Write Scope" }
                        }
                    }
                },
                Description = "JWT Authorization header using the Bearer scheme. Example: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = IdentityServerAuthenticationDefaults.AuthenticationScheme
                        }
                    },
                    new List<string>
                    {
                        "coding_hub_microservices_api.read",
                        "coding_hub_microservices_api.write"
                    }
                }
            });
        });
    }
}