using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using ZeroAlloc.Authorization;

namespace Rocket.Surgery.LaunchPad.Testing.Authorization;

/// <summary>
///     Access tokens for tests, signed with a key of this process. A web application under test trusts them in place of its identity provider
///     (<see cref="AddTestTokens" />), so the application's own JWT bearer authentication and authorization run as they do in production.
/// </summary>
public static class TestTokens
{
    /// <summary>The issuer of the test tokens.</summary>
    public const string Issuer = "https://tokens.launchpad.test";

    /// <summary>The audience of the test tokens: the one the samples expect.</summary>
    public const string Audience = "sample-api";

    /// <summary>A header that makes a request anonymous: no default test token is added to it.</summary>
    public const string AnonymousHeader = "X-Test-Anonymous";

    /// <summary>Every scope of the samples.</summary>
    public static readonly string[] AllScopes = ["rockets:read", "rockets:write", "launch-records:read", "launch-records:write"];

    private static readonly RsaSecurityKey Key = new(RSA.Create(2048)) { KeyId = "launchpad-test" };

    /// <summary>The token of the default test caller, who has every scope.</summary>
    public static string Default { get; } = Create("test-user", AllScopes);

    /// <summary>An access token for <paramref name="subject" /> with the given scopes.</summary>
    /// <param name="subject">The caller.</param>
    /// <param name="scopes">The scopes the token grants.</param>
    public static string Create(string subject, params string[] scopes) =>
        new JsonWebTokenHandler().CreateToken(
            new SecurityTokenDescriptor
            {
                Issuer = Issuer,
                Audience = Audience,
                Subject = new ClaimsIdentity([new Claim("sub", subject), new Claim("scope", string.Join(' ', scopes))]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new(Key, SecurityAlgorithms.RsaSha256),
            }
        );

    /// <summary>The <c>Authorization</c> header value for a token.</summary>
    /// <param name="token">The token.</param>
    public static string Bearer(string token) => "Bearer " + token;

    /// <summary>
    ///     Makes the application's JWT bearer authentication trust the test tokens instead of its identity provider, and gives a request
    ///     without an <c>Authorization</c> header the default test caller's token, unless it carries <see cref="AnonymousHeader" />.
    /// </summary>
    /// <param name="services">The application's services.</param>
    public static IServiceCollection AddTestTokens(this IServiceCollection services)
    {
        var configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
        configuration.SigningKeys.Add(Key);
        services.PostConfigureAll<JwtBearerOptions>(
            options =>
            {
                options.Authority = null;
                options.MetadataAddress = null!;
                options.RequireHttpsMetadata = false;
                options.Configuration = configuration;
                options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);
                options.TokenValidationParameters.ValidIssuer = Issuer;
                options.TokenValidationParameters.ValidAudience = Audience;
                options.TokenValidationParameters.IssuerSigningKey = Key;
            }
        );
        services.AddTransient<IStartupFilter, DefaultCallerFilter>();
        return services;
    }

    private sealed class DefaultCallerFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.Use(
                (context, following) =>
                {
                    var headers = context.Request.Headers;
                    if (headers.Authorization.Count == 0 && !headers.ContainsKey(AnonymousHeader)) headers.Authorization = Bearer(Default);
                    return following(context);
                }
            );
            next(app);
        };
    }
}

/// <summary>A caller for tests that dispatch requests without a transport: an identity with the scopes it is given.</summary>
/// <param name="id">The caller.</param>
/// <param name="scopes">The scopes the caller has.</param>
public sealed class TestCaller(string id, params string[] scopes) : ISecurityContext
{
    /// <summary>A caller with every scope of the samples.</summary>
    public static TestCaller Default { get; } = new("test-user", TestTokens.AllScopes);

    /// <inheritdoc />
    public string Id { get; } = id;

    /// <inheritdoc />
    public IReadOnlySet<string> Roles { get; } = new HashSet<string>(StringComparer.Ordinal);

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Claims { get; } = new Dictionary<string, string>(StringComparer.Ordinal) { ["scope"] = string.Join(' ', scopes) };
}
