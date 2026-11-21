using System.Globalization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Foundation;

using Problem = Rocket.Surgery.LaunchPad.Foundation.Problem;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

/// <summary>How an <see cref="Unauthorized" /> or <see cref="Forbidden" /> problem is written over HTTP: as the generated servers write the case.</summary>
[PublicAPI]
public static class AuthorizationProblems
{
    /// <summary>The status of the problem: 401 for <see cref="Unauthorized" />, otherwise 403.</summary>
    /// <param name="problem">The problem.</param>
    public static int StatusCode(Problem problem) => problem is Unauthorized ? StatusCodes.Status401Unauthorized : StatusCodes.Status403Forbidden;

    /// <summary>The problem as the problem details a union case answers with: the case's name in <c>case</c>, and its data.</summary>
    /// <param name="problem">The problem.</param>
    public static ProblemDetails Details(Problem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        var status = StatusCode(problem);
        var details = new ProblemDetails
        {
            Status = status,
            Title = problem.Title ?? ReasonPhrases.GetReasonPhrase(status),
            Detail = problem.Detail,
            Type = problem.Link,
            Instance = problem.Instance,
        };
        details.Extensions["case"] = problem.GetType().Name;
        foreach (var property in problem.Properties) details.Extensions[property.Key] = property.Value;
        return details;
    }

    /// <summary>
    ///     The <c>WWW-Authenticate</c> challenge of a denial (RFC 6750): the scopes to ask for, and <c>insufficient_scope</c> when the caller
    ///     is known and lacks one. Null when the denial names no scopes.
    /// </summary>
    /// <param name="decision">The denial.</param>
    /// <param name="existing">The challenge the authentication scheme already wrote, which is kept.</param>
    public static string? Challenge(AuthorizationDecision decision, string? existing = null)
    {
        if (decision.RequiredScopes.Count == 0) return existing;
        var scope = string.Create(CultureInfo.InvariantCulture, $"scope=\"{string.Join(' ', decision.RequiredScopes)}\"");
        if (decision.Problem is Forbidden forbidden)
        {
            return forbidden.Properties.ContainsKey(AuthorizationDecision.MissingScopesProperty) ? $"Bearer error=\"insufficient_scope\", {scope}" : existing;
        }

        if (string.IsNullOrWhiteSpace(existing)) return "Bearer " + scope;
        return  !existing.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase) || existing.Contains("scope=", StringComparison.OrdinalIgnoreCase) 
            ? existing
            :  existing.Trim().Length == "Bearer".Length ? "Bearer " + scope : existing + ", " + scope;
    }
}

/// <summary>
///     Answers what the authorization middleware rejected for a request's policies as the <see cref="Unauthorized" /> or
///     <see cref="Forbidden" /> case would be answered, after the authentication schemes challenged or forbade.
/// </summary>
internal sealed class RequestPoliciesResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _schemes = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        await _schemes.HandleAsync(next, context, policy, authorizeResult).ConfigureAwait(false);
        if (authorizeResult.Succeeded || context.Response.HasStarted || RequestPoliciesHandler.Decision(context) is not { Problem: { } problem } decision) return;
        // A scheme that redirects to sign in answered already.
        if (context.Response.StatusCode is not ( StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden )) return;

        if (AuthorizationProblems.Challenge(decision, context.Response.Headers.WWWAuthenticate) is { } challenge)
        {
            context.Response.Headers[HeaderNames.WWWAuthenticate] = challenge;
        }

        if (context.Request.ContentType?.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase) is true)
        {
            WriteGrpc(context, problem);
            return;
        }

        await Results.Problem(AuthorizationProblems.Details(problem)).ExecuteAsync(context).ConfigureAwait(false);
    }

    // A gRPC call that is rejected before it starts answers with its trailers as headers and no message: the status of the case, its name in
    // the case trailer, as the generated gRPC servers answer a union case, and the challenge beside them.
    private static void WriteGrpc(HttpContext context, Problem problem)
    {
        var response = context.Response;
        response.StatusCode = StatusCodes.Status200OK;
        response.ContentType = "application/grpc";
        response.Headers["grpc-status"] = problem is Unauthorized ? "16" : "7";
        response.Headers["grpc-message"] = Uri.EscapeDataString(problem.Detail ?? problem.GetType().Name);
        response.Headers["case"] = problem.GetType().Name;
    }
}

/// <summary>A denial the pipeline had to throw (a stream request) in a controller: answered as its case.</summary>
internal sealed class AuthorizationDeniedExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not AuthorizationDeniedException exception) return;
        context.ExceptionHandled = true;
        context.Result = new ObjectResult(AuthorizationProblems.Details(exception.Problem)) { StatusCode = AuthorizationProblems.StatusCode(exception.Problem) };
    }
}

/// <summary>A denial the pipeline had to throw (a stream request) in the exception handler: answered as its case.</summary>
internal sealed class AuthorizationDeniedProblemDetailsWriter : IProblemDetailsWriter
{
    public bool CanWrite(ProblemDetailsContext context) => context.Exception is AuthorizationDeniedException;

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        var problem = ( (AuthorizationDeniedException)context.Exception! ).Problem;
        return new(Results.Problem(AuthorizationProblems.Details(problem)).ExecuteAsync(context.HttpContext));
    }
}
