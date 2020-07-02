using System;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.SpaceShuttle.AspNetCore
{
    /// <summary>
    /// Default controller that integrates with <see cref="IMediator" />
    /// </summary>
    [PublicAPI]
    [ApiController]
    [ApiConventionType(typeof(MediatorApiConventions))]
    public abstract class MediatorApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mediator"></param>
        protected MediatorApiController(IMediator mediator) => _mediator = mediator;

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

            try
            {
                return await success(await _mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false))
                   .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (RequestException e)
            {
                return BadRequest(
                    ProblemDetailsFactory.CreateProblemDetails(
                        HttpContext,
                        StatusCodes.Status400BadRequest,
                        detail: e.Message
                    )
                );
            }
            catch (ValidationException e)
            {
                HttpContext.Items[typeof(ValidationException)] = e;
                return UnprocessableEntity(
                    ProblemDetailsFactory.CreateValidationProblemDetails(
                        HttpContext,
                        ModelState,
                        StatusCodes.Status422UnprocessableEntity
                    )
                );
            }
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

            try
            {
                await _mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
                return await success().ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (RequestException e)
            {
                return BadRequest(
                    ProblemDetailsFactory.CreateProblemDetails(
                        HttpContext,
                        StatusCodes.Status400BadRequest,
                        detail: e.Message
                    )
                );
            }
            catch (ValidationException e)
            {
                HttpContext.Items[typeof(ValidationException)] = e;
                return UnprocessableEntity(
                    ProblemDetailsFactory.CreateValidationProblemDetails(
                        HttpContext,
                        ModelState,
                        StatusCodes.Status422UnprocessableEntity
                    )
                );
            }
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

            try
            {
                return success(await _mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (RequestException e)
            {
                return BadRequest(
                    ProblemDetailsFactory.CreateProblemDetails(
                        HttpContext,
                        StatusCodes.Status400BadRequest,
                        detail: e.Message
                    )
                );
            }
            catch (ValidationException e)
            {
                HttpContext.Items[typeof(ValidationException)] = e;
                return UnprocessableEntity(
                    ProblemDetailsFactory.CreateValidationProblemDetails(
                        HttpContext,
                        ModelState,
                        StatusCodes.Status422UnprocessableEntity
                    )
                );
            }
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

            try
            {
                await _mediator.Send(request, HttpContext.RequestAborted).ConfigureAwait(false);
                return success();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (RequestException e)
            {
                return BadRequest(
                    ProblemDetailsFactory.CreateProblemDetails(
                        HttpContext,
                        StatusCodes.Status400BadRequest,
                        detail: e.Message
                    )
                );
            }
            catch (ValidationException e)
            {
                HttpContext.Items[typeof(ValidationException)] = e;
                return UnprocessableEntity(
                    ProblemDetailsFactory.CreateValidationProblemDetails(
                        HttpContext,
                        ModelState,
                        StatusCodes.Status422UnprocessableEntity
                    )
                );
            }
        }
    }
}