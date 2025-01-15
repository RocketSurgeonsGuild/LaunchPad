using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ProblemDetailsConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention<AspNetCoreConvention>]
[ConventionCategory(ConventionCategory.Application)]
[SuppressMessage("Trimming", "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code", Justification = "Mvc pieces are not specifically added, and problem writers are used by other apis")]
public class ProblemDetailsConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler(
            options =>
            {
                var old = options.StatusCodeSelector;
                options.StatusCodeSelector = exception => exception switch
                                                          {
                                                              NotFoundException => StatusCodes.Status404NotFound,
                                                              RequestFailedException => StatusCodes.Status400BadRequest,
                                                              NotAuthorizedException => StatusCodes.Status403Forbidden,
                                                              ValidationException => StatusCodes.Status422UnprocessableEntity,
                                                              _ => old?.Invoke(exception) ?? StatusCodes.Status500InternalServerError,
                                                          };
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, OnBeforeWriteProblemDetailsWriter>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, FluentValidationProblemDetailsWriter>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, ValidationExceptionProblemDetailsWriter>());

        services
           .AddOptions<ApiBehaviorOptions>()
           .Configure(static options => options.SuppressModelStateInvalidFilter = true);
    }
}

internal class OnBeforeWriteProblemDetailsWriter(IOptions<ApiBehaviorOptions> apiBehaviorOptions) : IProblemDetailsWriter
{
    public ValueTask WriteAsync(ProblemDetailsContext context) => throw new NotImplementedException();

    public bool CanWrite(ProblemDetailsContext context)
    {
        if (!context.ProblemDetails.Status.HasValue
         || !apiBehaviorOptions.Value.ClientErrorMapping.TryGetValue(context.ProblemDetails.Status.Value, out var clientErrorData))
        {
            return false;
        }

        context.ProblemDetails.Title ??= clientErrorData.Title;
        context.ProblemDetails.Type ??= clientErrorData.Link;
        return false;
    }
}

internal class FluentValidationProblemDetailsWriter(IOptions<ApiBehaviorOptions> apiBehaviorOptions) : IProblemDetailsWriter
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

internal class ValidationExceptionProblemDetailsWriter(IOptions<ApiBehaviorOptions> apiBehaviorOptions) : IProblemDetailsWriter
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
