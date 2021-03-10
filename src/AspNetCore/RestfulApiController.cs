using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    /// <summary>
    /// Default controller that integrates with <see cref="IMediator" />
    /// </summary>
    [PublicAPI]
    [ApiController]
    public abstract class RestfulApiController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        /// <summary>
        /// Send an request and allow for async <see cref="ActionResult{TResponse}" />
        /// </summary>
        /// <typeparam name="TResponse">The response model</typeparam>
        /// <param name="request">The request model</param>
        /// <param name="success">The method to call when the request succeeds</param>
        protected async Task<ActionResult<TResponse>> Send<TResponse>(
            IRequest<TResponse> request,
            Func<TResponse, Task<ActionResult<TResponse>>> success
        )
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (success is null)
            {
                throw new ArgumentNullException(nameof(success));
            }

            return await success(await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false))
               .ConfigureAwait(false);
        }

        /// <summary>
        /// Send an request and allow for sync <see cref="ActionResult{TResponse}" />
        /// </summary>
        /// <typeparam name="TResponse">The response model</typeparam>
        /// <param name="request">The request model</param>
        /// <param name="success">The method to call when the request succeeds</param>
        protected async Task<ActionResult<TResponse>> Send<TResponse>(
            IRequest<TResponse> request,
            Func<TResponse, ActionResult<TResponse>> success
        )
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (success is null)
            {
                throw new ArgumentNullException(nameof(success));
            }

            return success(await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false));
        }

        /// <summary>
        /// Send an request and allow for async <see cref="ActionResult" />
        /// </summary>
        /// <param name="request">The request model</param>
        /// <param name="success">The method to call when the request succeeds</param>
        protected async Task<ActionResult> Send<TResponse>(
            IRequest<TResponse> request,
            Func<Task<ActionResult>> success
        )
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (success is null)
            {
                throw new ArgumentNullException(nameof(success));
            }

            await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return await success().ConfigureAwait(false);
        }

        /// <summary>
        /// Send an request and allow for sync <see cref="ActionResult" />
        /// </summary>
        /// <param name="request">The request model</param>
        /// <param name="success">The method to call when the request succeeds</param>
        protected async Task<ActionResult> Send<TResponse>(IRequest<TResponse> request, Func<ActionResult> success)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (success is null)
            {
                throw new ArgumentNullException(nameof(success));
            }

            await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return success();
        }

        /// <summary>
        /// Send an request and allow for sync <see cref="ActionResult" />
        /// </summary>
        /// <param name="request">The request model</param>
        protected async Task<ActionResult> Send(IRequest<Unit> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await Mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return NoContent();
        }
    }
}