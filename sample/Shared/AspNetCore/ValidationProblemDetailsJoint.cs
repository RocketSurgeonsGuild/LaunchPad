using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

using ZeroAlloc.Mediator.Validation;

namespace Sample.Shared;

/// <summary>
///     The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.
/// </summary>
public class ValidationFailureProblemDetails : ProblemDetails
{
    /// <summary>
    ///     Creates the problem details for the failures of a <see cref="ValidationFailedException" />.
    /// </summary>
    public static ValidationFailureProblemDetails From(ValidationFailedException exception) =>
        new()
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "One or more validation errors occurred.",
            Errors = exception
                    .Error.Failures
                    .GroupBy(failure => failure.PropertyName, StringComparer.Ordinal)
                    .ToDictionary(
                         group => group.Key,
                         group => group.Select(failure => new ValidationFailureDetail(failure.PropertyName, failure.ErrorMessage, failure.ErrorCode)).ToArray(),
                         StringComparer.Ordinal
                     ),
        };

    /// <summary>
    ///     The failures, by property name.
    /// </summary>
    public IDictionary<string, ValidationFailureDetail[]> Errors { get; init; } = new Dictionary<string, ValidationFailureDetail[]>(StringComparer.Ordinal);
}

/// <summary>
///     A single validation failure.
/// </summary>
/// <param name="PropertyName">The property that failed.</param>
/// <param name="ErrorMessage">Why it failed.</param>
/// <param name="ErrorCode">The code of the rule that failed.</param>
public record ValidationFailureDetail(string PropertyName, string ErrorMessage, string? ErrorCode);

/// <summary>
///     Turns the <see cref="ValidationFailedException" /> the ZeroAlloc mediator throws for an invalid request into a 422 with
///     <see cref="ValidationFailureProblemDetails" />, for controllers and for the exception handler alike.
/// </summary>
[DovetailExport]
[AfterJoint<ProblemDetailsJoint>]
public class ValidationProblemDetailsJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddExceptionHandler(
            options =>
            {
                var old = options.StatusCodeSelector;
                options.StatusCodeSelector = exception => exception is ValidationFailedException
                    ? StatusCodes.Status422UnprocessableEntity
                    : old?.Invoke(exception) ?? StatusCodes.Status500InternalServerError;
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, ValidationFailureProblemDetailsWriter>());
        services
           .Configure<MvcOptions>(options => options.Filters.Insert(0, new ValidationFailureExceptionFilter()))
           .Configure<RestfulApiOptions>(options => options.ValidationProblemDetailsType = typeof(ValidationFailureProblemDetails));
    }

    private sealed class ValidationFailureExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not ValidationFailedException exception) return;
            context.ExceptionHandled = true;
            context.Result = new UnprocessableEntityObjectResult(ValidationFailureProblemDetails.From(exception));
        }
    }

    private sealed class ValidationFailureProblemDetailsWriter : IProblemDetailsWriter
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
