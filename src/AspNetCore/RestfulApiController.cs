using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Default controller that integrates with <see cref="IMediator" />
/// </summary>
[PublicAPI]
[ApiController]
public abstract class RestfulApiController : ControllerBase
{
    private IMediator? _mediator;

    /// <summary>
    ///     The mediator instance available on demand
    /// </summary>
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    ///     Send an request and allow for async <see cref="ActionResult{TResponse}" />
    /// </summary>
    /// <typeparam name="TResponse">The response model</typeparam>
    /// <param name="request">The request model</param>
    /// <param name="success">The method to call when the request succeeds</param>
    protected async Task<ActionResult<TResponse>> Send<TResponse>(
        IRequest<TResponse> request,
        Func<TResponse, Task<ActionResult<TResponse>>> success
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentNullException.ThrowIfNull(success);

        return await success(await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false))
           .ConfigureAwait(false);
    }

    /// <summary>
    ///     Send an request and allow for sync <see cref="ActionResult{TResponse}" />
    /// </summary>
    /// <typeparam name="TResponse">The response model</typeparam>
    /// <param name="request">The request model</param>
    /// <param name="success">The method to call when the request succeeds</param>
    protected async Task<ActionResult<TResponse>> Send<TResponse>(
        IRequest<TResponse> request,
        Func<TResponse, ActionResult<TResponse>> success
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentNullException.ThrowIfNull(success);

        return success(await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false));
    }

    /// <summary>
    ///     Send an request and allow for sync <see cref="ActionResult" />
    /// </summary>
    /// <param name="request">The request model</param>
    /// <param name="success">The method to call when the request succeeds</param>
    protected async Task<ActionResult> Send<TResponse>(IRequest<TResponse> request, Func<ActionResult> success)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentNullException.ThrowIfNull(success);

        await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return success();
    }

    /// <summary>
    ///     Send an request and allow for sync <see cref="ActionResult" />
    /// </summary>
    /// <param name="request">The request model</param>
    protected async Task<ActionResult> Send(IRequest<Unit> request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
        return NoContent();
    }
}
