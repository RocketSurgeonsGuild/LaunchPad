using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Sample.Core.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     How the samples with pages know their callers: a caller signs in with the identity provider named by
///     <c>Authentication:Authority</c> (OpenID Connect, authorization code with PKCE) and is remembered in a cookie. The scopes the caller
///     granted become the <c>scope</c> claim, which the policies of the operations (Sample.Core) require. Sample.AppHost runs Keycloak and
///     sets the authority; without one nobody can sign in, and the operations answer that the caller is unauthorized.
/// </summary>
internal static class SampleSignIn
{
    /// <summary>The public client of the samples in the identity provider's realm.</summary>
    public const string ClientId = "sample-web";

    public static IServiceCollection AddSampleSignIn(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Authentication:Authority"]?.TrimEnd('/');
        var authentication = services
                            .AddAuthentication(
                                 options =>
                                 {
                                     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                     if (authority is { }) options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                                 }
                             )
                            .AddCookie();
        if (authority is null) return services;

        authentication.AddOpenIdConnect(
            options =>
            {
                options.Authority = authority;
                options.ClientId = configuration["Authentication:ClientId"] ?? ClientId;
                options.ResponseType = "code";
                options.UsePkce = true;
                options.SaveTokens = true;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                // The AppHost's Keycloak listens on HTTP.
                options.RequireHttpsMetadata = !authority.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                foreach (var scope in Scopes.All.Keys) options.Scope.Add(scope);
                // The scopes that were granted are in the token response, not in the identity token the caller's claims come from.
                options.Events.OnTokenValidated = context =>
                {
                    if (context is { TokenEndpointResponse.Scope: { Length: > 0 } granted, Principal.Identity: ClaimsIdentity identity })
                    {
                        identity.AddClaim(new("scope", granted));
                    }

                    return Task.CompletedTask;
                };
            }
        );
        return services;
    }

    /// <summary>Maps <c>/sign-in</c> and <c>/sign-out</c>.</summary>
    public static IEndpointRouteBuilder MapSampleSignIn(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/sign-in",
            (string? returnUrl) => TypedResults.Challenge(new AuthenticationProperties { RedirectUri = Local(returnUrl) })
        );
        endpoints.MapGet(
            "/sign-out",
            async (HttpContext context) =>
            {
                // Only the schemes that are registered: there is no identity provider to sign out of without an authority.
                var schemes = await context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>().GetAllSchemesAsync();
                return TypedResults.SignOut(new() { RedirectUri = "/" }, [.. schemes.Select(s => s.Name)]);
            }
        );
        return endpoints;
    }

    // Only a path of this application: a return address is never taken from the caller as it is.
    private static string Local(string? returnUrl) =>
        returnUrl is ['/', ..] && !returnUrl.StartsWith("//", StringComparison.Ordinal) && !returnUrl.StartsWith("/\\", StringComparison.Ordinal) ? returnUrl : "/";
}
