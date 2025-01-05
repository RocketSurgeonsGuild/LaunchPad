
using MediatR;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A common handler for creating patch methods
/// </summary>
/// <typeparam name="TRequest">The request type that will be run after the patch has been applied</typeparam>
/// <typeparam name="TPatch">The patch object itself</typeparam>
/// <typeparam name="TResponse">The final result object</typeparam>
/// <remarks>
///     The based handler using Mediator
/// </remarks>
/// <param name="mediator"></param>
public abstract class PatchRequestHandler<TRequest, TPatch, TResponse>(IMediator mediator) : IRequestHandler<TPatch, TResponse>
    where TRequest : IRequest<TResponse>
    where TPatch : IPropertyTracking<TRequest>, IRequest<TResponse>
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    ///     Method used to get request database calls, etc.
    /// </summary>
    /// <param name="patchRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TRequest> GetRequest(TPatch patchRequest, CancellationToken cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TResponse> Handle(TPatch request, CancellationToken cancellationToken)
    {
        var underlyingRequest = await GetRequest(request, cancellationToken);
        return await _mediator.Send(request.ApplyChanges(underlyingRequest), cancellationToken);
    }
}
