using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation;

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
        services
           .AddOptions<ProblemDetailsOptions>()
           .Configure<IOptions<ApiBehaviorOptions>>(
                static (options, apiBehaviorOptions) =>
                {
                    var previous = options.CustomizeProblemDetails;
                    options.CustomizeProblemDetails = context =>
                                                      {
                                                          ApplyClientErrorMapping(apiBehaviorOptions.Value, context.ProblemDetails);
                                                          previous?.Invoke(context);
                                                      };
                }
            );

        services
           .AddOptions<ApiBehaviorOptions>()
           .Configure(static options => options.SuppressModelStateInvalidFilter = true);
    }

    /// <summary>
    ///     Fills in the title and the type of problem details from the client error mapping of <see cref="ApiBehaviorOptions" />, when its status has one
    ///     and they are not already set.
    /// </summary>
    internal static void ApplyClientErrorMapping(ApiBehaviorOptions apiBehaviorOptions, ProblemDetails problemDetails)
    {
        if (!problemDetails.Status.HasValue
         || !apiBehaviorOptions.ClientErrorMapping.TryGetValue(problemDetails.Status.Value, out var clientErrorData))
        {
            return;
        }

        problemDetails.Title ??= clientErrorData.Title;
        problemDetails.Type ??= clientErrorData.Link;
    }
}
