using Microsoft.Extensions.DependencyInjection;

using ZeroAlloc.Authorization;
using ZeroAlloc.Mediator;

// The parameter order is ZeroAlloc.Mediator's contract for a pipeline behavior.
#pragma warning disable CA1068

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     The ZeroAlloc.Mediator pipeline behavior that checks a request's policies before anything else runs. A caller with no identity answers
///     the <see cref="Unauthorized" /> case of the response union, and a caller a policy denies answers <see cref="Forbidden" />; a request
///     without policies passes straight through.
/// </summary>
/// <remarks>
///     <para>
///         The plan of a request, and how a denial becomes its response, are registered with the services of the application: LaunchPad's
///         generator writes <c>AddLaunchPadRequestAuthorization()</c> for each project, and the joint LaunchPad writes beside it applies it.
///         A request that has a policy attribute and no plan is not let through: it throws, naming the registration that is missing.
///     </para>
///     <para>
///         The services are the ones the mediator dispatches the request with: <c>IMediator</c>, resolved from the scope of the call.
///         ZeroAlloc.Mediator's static <c>Mediator</c> class dispatches without any, so LaunchPad doesn't support it and bans it
///         (RS0030); a request with a policy that is dispatched that way is refused.
///     </para>
///     <para>
///         Policies the edge already decided for this call are not run again: their outcomes are in the call's
///         <see cref="AuthorizationOutcomes" />. A stream request has no response to answer with, so its denial throws
///         <see cref="AuthorizationDeniedException" />.
///     </para>
/// </remarks>
[PublicAPI]
[PipelineBehavior(Order = -1000)]
public sealed class AuthorizationBehavior : IPipelineBehavior
{
    /// <summary>
    ///     What ZeroAlloc.Mediator's static <c>Mediator</c> class calls, which dispatches with no services: its generator writes the call for
    ///     every behavior, so the overload has to be there. LaunchPad doesn't support that way of dispatching: a request with a policy is
    ///     refused, and any other request passes on.
    /// </summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <typeparam name="TResponse">The response.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="ct">The cancellation token of the request.</param>
    /// <param name="next">The rest of the pipeline.</param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next
    )
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(next);
        _ = Plan<TRequest>(null);
        return next(request, ct);
    }

    /// <summary>Checks the request's policies with the services of the request being dispatched.</summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <typeparam name="TResponse">The response.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="services">The services of the request being dispatched.</param>
    /// <param name="ct">The cancellation token of the request.</param>
    /// <param name="next">The rest of the pipeline.</param>
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(
        TRequest request,
        IServiceProvider services,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next
    )
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(next);
        return Plan<TRequest>(services) is { } plan ? AuthorizeAsync(plan, request, services, ct, next) : next(request, ct);
    }

    /// <summary>Checks a stream request's policies before its handler runs; a denial throws <see cref="AuthorizationDeniedException" />.</summary>
    /// <typeparam name="TRequest">The stream request.</typeparam>
    /// <param name="request">The stream request.</param>
    /// <param name="services">The services of the request being dispatched.</param>
    /// <param name="ct">The cancellation token of the enumeration.</param>
    public static async ValueTask CheckStream<TRequest>(TRequest request, IServiceProvider? services, CancellationToken ct)
    {
        if (Plan<TRequest>(services) is not { } plan) return;
        var decision = await services!.GetRequiredService<RequestAuthorizer>().AuthorizeAsync(plan, ct).ConfigureAwait(false);
        if (decision.Problem is { } problem) throw new AuthorizationDeniedException(problem);
    }

    private static async ValueTask<TResponse> AuthorizeAsync<TRequest, TResponse>(
        RequestAuthorizationPlan<TRequest> plan,
        TRequest request,
        IServiceProvider services,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next
    )
    {
        var decision = await services.GetRequiredService<RequestAuthorizer>().AuthorizeAsync(plan, ct).ConfigureAwait(false);
        return decision.Problem is not { } problem
            ? await next(request, ct).ConfigureAwait(false)
            : (problem, services.GetService<AuthorizationResponse<TResponse>>()) switch
            {
                (Unauthorized unauthorized, { } response) => response.Unauthorized(unauthorized),
                (Forbidden forbidden, { } response) => response.Forbidden(forbidden),
                _ => throw new AuthorizationDeniedException(problem),
            };
    }

    // The plan of the request, from the services of the call; null for a request without policies. A request with a policy attribute is
    // never let through because its plan was not registered, or because there are no services to find it in.
    private static RequestAuthorizationPlan<TRequest>? Plan<TRequest>(IServiceProvider? services) =>
        services?.GetService<RequestAuthorizationPlan<TRequest>>()
     ?? ( HasPolicies<TRequest>.Value
            ? throw new InvalidOperationException(
                services is null
                    ? $"{typeof(TRequest)} has policies, and was dispatched without the services of a call to check them with: through ZeroAlloc.Mediator's static Mediator class. Dispatch it through IMediator resolved from a service scope."
                    : $"{typeof(TRequest)} has policies, but no authorization plan is registered for it. Apply the joints of its assembly (ConfigureDovetail), or call AddLaunchPadRequestAuthorization() of that assembly on the services."
            )
            : null );

    // Read once for each request type, and only to refuse a request whose plan is missing.
    private static class HasPolicies<TRequest>
    {
        public static readonly bool Value = typeof(TRequest).IsDefined(typeof(RequirePolicyAttribute), false) || typeof(TRequest).IsDefined(typeof(RequireAnyPolicyAttribute), false);
    }
}
