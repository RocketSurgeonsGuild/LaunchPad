using FluentValidation;
using FluentValidation.Results;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Maps the FluentValidation <see cref="ValidationException" /> to an unprocessable entity, and registers the problem details writers that
///     describe validation failures with <see cref="FluentValidationProblemDetails" />.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Runs after the <see cref="ProblemDetailsJoint" />, so the writers come after the problem details writers it registers, and the status code
///     selector it configures is kept for every other exception.
/// </remarks>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="ProblemDetailsJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<ProblemDetailsJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class FluentValidationProblemDetailsJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddExceptionHandler(
            options =>
            {
                var old = options.StatusCodeSelector;
                options.StatusCodeSelector = exception => exception switch
                                                          {
                                                              ValidationException => StatusCodes.Status422UnprocessableEntity,
                                                              _ => old?.Invoke(exception) ?? StatusCodes.Status500InternalServerError,
                                                          };
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, FluentValidationProblemDetailsWriter>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, ValidationExceptionProblemDetailsWriter>());
    }
}

internal class FluentValidationProblemDetailsWriter : IProblemDetailsWriter
{
    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        if (context is not { Exception: IProblemDetailsData details }
         || context.HttpContext.Items[typeof(ValidationResult)] is not ValidationResult validationResult)
        {
            return ValueTask.CompletedTask;
        }

        context.ProblemDetails = new FluentValidationProblemDetails(validationResult.Errors)
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = details.Title ?? context.ProblemDetails.Title,
            Type = details.Link ?? context.ProblemDetails.Type,
            Detail = context.ProblemDetails.Detail,
            Instance = details.Instance ?? context.ProblemDetails.Instance,
            Extensions = context.ProblemDetails.Extensions,
        };
        return ValueTask.CompletedTask;
    }

    public bool CanWrite(ProblemDetailsContext context) => context.Exception is not IProblemDetailsData && context.HttpContext.Items[typeof(ValidationResult)] is ValidationResult;
}

internal class ValidationExceptionProblemDetailsWriter : IProblemDetailsWriter
{
    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        if (context.Exception is not ValidationException validationException) return ValueTask.CompletedTask;
        context.ProblemDetails = new FluentValidationProblemDetails(validationException.Errors)
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = context.ProblemDetails.Title,
            Type = context.ProblemDetails.Type,
            Detail = context.ProblemDetails.Detail,
            Instance = context.ProblemDetails.Instance,
            Extensions = context.ProblemDetails.Extensions,
        };
        return ValueTask.CompletedTask;
    }

    public bool CanWrite(ProblemDetailsContext context) => context.Exception is ValidationException;
}
