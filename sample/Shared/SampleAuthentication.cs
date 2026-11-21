using Microsoft.AspNetCore.Authentication.JwtBearer;

using Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

using Sample.Core.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     How the API samples know their callers: access tokens (JWTs) of the identity provider named by <c>Authentication:Authority</c>, issued
///     for the audience <c>Authentication:Audience</c>. Sample.AppHost runs Keycloak and sets both; the policies of the operations
///     (Sample.Core) then require the scopes the token carries.
/// </summary>
internal static class SampleAuthentication
{
    /// <summary>The audience the samples' identity provider issues their access tokens for.</summary>
    public const string DefaultAudience = "sample-api";

    public static IServiceCollection AddSampleAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Authentication:Authority"]?.TrimEnd('/');
        services
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(
                options =>
                {
                    options.Authority = authority;
                    options.Audience = configuration["Authentication:Audience"] ?? DefaultAudience;
                    // The claims as the token names them: scope stays scope, sub stays sub.
                    options.MapInboundClaims = false;
                    // The AppHost's Keycloak listens on HTTP.
                    options.RequireHttpsMetadata = authority?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) is not true;
                }
            );

        // The OpenAPI document declares the scheme its operations require their scopes of, so an API explorer can sign in. Keycloak's
        // endpoints are under the realm; without an authority (a document written at build) LaunchPad declares a bearer token instead.
        services.Configure<OpenApiAuthorizationOptions>(
            options =>
            {
                foreach (var (scope, description) in Scopes.All) options.ScopeDescriptions[scope] = description;
                if (authority is null) return;
                options.AuthorizationUrl = new($"{authority}/protocol/openid-connect/auth");
                options.TokenUrl = new($"{authority}/protocol/openid-connect/token");
            }
        );
        return services;
    }
}
