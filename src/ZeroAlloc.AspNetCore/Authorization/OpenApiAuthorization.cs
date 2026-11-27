using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

using Rocket.Surgery.LaunchPad.Authorization;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

/// <summary>How the scopes of an operation's policies are described in the OpenAPI document.</summary>
[PublicAPI]
public sealed class OpenApiAuthorizationOptions
{
    /// <summary>
    ///     The name of the security scheme the operations require their scopes of. The application declares the scheme, or sets
    ///     <see cref="AuthorizationUrl" /> and <see cref="TokenUrl" /> for LaunchPad to declare it as OAuth 2.0; with neither, LaunchPad
    ///     declares it as a bearer token.
    /// </summary>
    public string SchemeName { get; set; } = "oauth2";

    /// <summary>The authorization endpoint of the identity provider. With <see cref="TokenUrl" />, LaunchPad declares the scheme.</summary>
    public Uri? AuthorizationUrl { get; set; }

    /// <summary>The token endpoint of the identity provider. With <see cref="AuthorizationUrl" />, LaunchPad declares the scheme.</summary>
    public Uri? TokenUrl { get; set; }

    /// <summary>What each scope grants, by scope, for the scheme LaunchPad declares. A scope without one is described by its own name.</summary>
    public IDictionary<string, string> ScopeDescriptions { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
}

/// <summary>What the generated servers' OpenAPI transformers call to describe the authorization of an operation.</summary>
[PublicAPI]
public static class OpenApiAuthorization
{
    /// <summary>
    ///     States the scopes an operation requires, from the plan of the request it dispatches: a security requirement per alternative, any
    ///     one of which is enough (docs/design/authorization.md, section 5). A request without a plan adds nothing.
    /// </summary>
    /// <typeparam name="TRequest">The request the operation dispatches.</typeparam>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context of the operation's transformer.</param>
    public static void RequireScopes<TRequest>(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);
        if (context.ApplicationServices.GetService<RequestAuthorizationPlan<TRequest>>() is not { } plan) return;

        var scheme = context.ApplicationServices.GetRequiredService<IOptions<OpenApiAuthorizationOptions>>().Value.SchemeName;
        foreach (var alternative in plan.ScopeAlternatives)
        {
            operation.Security ??= [];
            operation.Security.Add(new() { [new OpenApiSecuritySchemeReference(scheme, context.Document)] = [.. alternative] });
        }
    }
}

/// <summary>
///     Declares the security scheme the operations require their scopes of, unless the application declared one of that name: OAuth 2.0 with
///     the authorization code flow and every scope the application's requests require when the application gave both endpoints, otherwise
///     a bearer token. A requirement of a
///     scheme the document doesn't declare is written as <c>{}</c>, which reads as "no security", so the scheme is never left out.
/// </summary>
internal sealed class RequestPoliciesSecuritySchemeTransformer(IOptions<OpenApiAuthorizationOptions> options, RequestAuthorizationPlans plans) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (document.Components?.SecuritySchemes?.ContainsKey(settings.SchemeName) is true) return Task.CompletedTask;

        var scopes = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var scope in plans.All.SelectMany(p => p.ScopeAlternatives).SelectMany(a => a))
        {
            scopes[scope] = settings.ScopeDescriptions.TryGetValue(scope, out var text) ? text : scope;
        }

        if (plans.All.Count == 0) return Task.CompletedTask;

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.Ordinal);
        document.Components.SecuritySchemes[settings.SchemeName] = settings is { AuthorizationUrl: { } authorizationUrl, TokenUrl: { } tokenUrl }
            ? new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new()
                {
                    AuthorizationCode = new()
                    {
                        AuthorizationUrl = authorizationUrl,
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>(scopes, StringComparer.Ordinal),
                    },
                },
            }
            : new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "An access token with the scopes the operation requires.",
            };
        return Task.CompletedTask;
    }
}
