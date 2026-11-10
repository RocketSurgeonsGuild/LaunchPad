using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using ZeroAlloc.Mediator.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Turns the <see cref="ValidationFailedException" /> the ZeroAlloc mediator throws for an invalid request into a 422 with
///     <see cref="ValidationFailureProblemDetails" />, for controllers and for the exception handler alike.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Runs after the <see cref="ProblemDetailsJoint" />, so the status code selector it configures is kept for every other exception.
/// </remarks>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="ProblemDetailsJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<ProblemDetailsJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocProblemDetailsJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddExceptionHandler(
            options =>
            {
                var previous = options.StatusCodeSelector;
                options.StatusCodeSelector = exception => exception is ValidationFailedException
                    ? StatusCodes.Status422UnprocessableEntity
                    : previous?.Invoke(exception) ?? StatusCodes.Status500InternalServerError;
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, ValidationFailureProblemDetailsWriter>());
        services.Configure<MvcOptions>(options => options.Filters.Insert(0, new ValidationFailureExceptionFilter()));
    }

    internal sealed class ValidationFailureExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not ValidationFailedException exception) return;
            context.ExceptionHandled = true;
            context.Result = new UnprocessableEntityObjectResult(ValidationFailureProblemDetails.From(exception));
        }
    }

    internal sealed class ValidationFailureProblemDetailsWriter : IProblemDetailsWriter
    {
        public bool CanWrite(ProblemDetailsContext context) => context.Exception is ValidationFailedException;

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            var details = ValidationFailureProblemDetails.From((ValidationFailedException)context.Exception!);
            details.Instance = context.ProblemDetails.Instance;
            details.Extensions = context.ProblemDetails.Extensions;
            context.HttpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return new(context.HttpContext.Response.WriteAsJsonAsync(details));
        }
    }
}
