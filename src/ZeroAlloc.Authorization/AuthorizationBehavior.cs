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
///     Policies the edge already decided for this call are not run again: their outcomes are in the call's
///     <see cref="AuthorizationOutcomes" />. A stream request has no response to answer with, so its denial throws
///     <see cref="AuthorizationDeniedException" />.
/// </remarks>
[PublicAPI]
[PipelineBehavior(Order = -1000)]
public sealed class AuthorizationBehavior : IPipelineBehavior
{
    /// <summary>Checks the request's policies with the services of the request being dispatched.</summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <typeparam name="TResponse">The response.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="ct">The cancellation token of the request.</param>
    /// <param name="next">The rest of the pipeline.</param>
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next
    )
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(next);
        return RequestAuthorizationPlan<TRequest>.Instance is { } plan
            ? AuthorizeAsync(plan, request, Services(PipelineBehaviorServices.Current, plan), ct, next)
            : next(request, ct);
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
        return RequestAuthorizationPlan<TRequest>.Instance is { } plan
            ? AuthorizeAsync(plan, request, Services(services, plan), ct, next)
            : next(request, ct);
    }

    /// <summary>Checks a stream request's policies before its handler runs; a denial throws <see cref="AuthorizationDeniedException" />.</summary>
    /// <typeparam name="TRequest">The stream request.</typeparam>
    /// <param name="request">The stream request.</param>
    /// <param name="services">The services of the request being dispatched.</param>
    /// <param name="ct">The cancellation token of the enumeration.</param>
    public static async ValueTask CheckStream<TRequest>(TRequest request, IServiceProvider? services, CancellationToken ct)
    {
        if (RequestAuthorizationPlan<TRequest>.Instance is not { } plan) return;
        var decision = await RequestAuthorizer.AuthorizeAsync(plan, request, Services(services ?? PipelineBehaviorServices.Current, plan), ct).ConfigureAwait(false);
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
        var decision = await RequestAuthorizer.AuthorizeAsync(plan, request, services, ct).ConfigureAwait(false);
        return decision.Problem switch
        {
            null => await next(request, ct).ConfigureAwait(false),
            Unauthorized unauthorized when AuthorizationResponse<TResponse>.Unauthorized is { } answer => answer(unauthorized),
            Forbidden forbidden when AuthorizationResponse<TResponse>.Forbidden is { } answer => answer(forbidden),
            var problem => throw new AuthorizationDeniedException(problem),
        };
    }

    // A request with policies is never let through because nothing could decide them.
    private static IServiceProvider Services(IServiceProvider? services, RequestAuthorizationPlan plan) =>
        services
     ?? throw new InvalidOperationException(
            $"{plan.Id} has policies, and was dispatched without the services of a call to check them with. Dispatch it through IMediator resolved from a service scope."
        );
}
