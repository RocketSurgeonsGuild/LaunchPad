using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers problem details and the exception handler status codes for the LaunchPad exceptions.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
[SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Mvc pieces are not specifically added, and problem writers are used by other apis"
)]
public class ProblemDetailsJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
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
                                                              _ => old?.Invoke(exception) ?? StatusCodes.Status500InternalServerError,
                                                          };
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, OnBeforeWriteProblemDetailsWriter>());

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
