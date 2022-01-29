using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Default page model with Mediator made available
/// </summary>
public class MediatorPageModel : PageModel
{
    private IMediator? _mediator;

    /// <summary>
    ///     The mediator instance
    /// </summary>
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    ///     The mediator send method
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    protected Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken? cancellationToken = default)
    {
        return Mediator.Send(request, cancellationToken ?? HttpContext.RequestAborted);
    }
}
