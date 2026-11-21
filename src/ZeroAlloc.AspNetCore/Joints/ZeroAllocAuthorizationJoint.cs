using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore.Authorization;
using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Foundation;

using ZeroAlloc.Authorization;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Checks the policies of a request at the edge of an ASP.NET Core application, before it is bound, and describes them in the OpenAPI
///     document (docs/design/authorization.md, sections 4 and 5).
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         The caller policies see is the one ASP.NET Core authenticated, unless the application registered its own
///         <see cref="ISecurityContext" />. The generated servers name a policy of the form <c>launchpad:{request}</c> on an operation whose
///         request has policies; it runs the request's plan and records the outcomes the mediator pipeline then reuses. What it rejects is
///         answered as the <see cref="Unauthorized" /> or <see cref="Forbidden" /> case, with the scopes in <c>WWW-Authenticate</c>.
///     </para>
///     <para>
///         An application that registers its own <see cref="IAuthorizationPolicyProvider" /> or
///         <see cref="IAuthorizationMiddlewareResultHandler" /> after this joint replaces LaunchPad's, and with it the edge.
///     </para>
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<OpenApiJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocAuthorizationJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddAuthorization();
        services.AddOptions<RequestAuthorizationOptions>();
        services.AddOptions<OpenApiAuthorizationOptions>();
        services.TryAddScoped<AuthorizationOutcomes>();
        services.TryAddScoped<ISecurityContext, HttpSecurityContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, RequestPolicyProvider>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, RequestPoliciesResultHandler>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, RequestPoliciesHandler>());

        // A denial the pipeline throws (a stream request has no response to answer with) answers as its case.
        services.AddExceptionHandler(
            options =>
            {
                var previous = options.StatusCodeSelector;
                options.StatusCodeSelector = exception => exception is AuthorizationDeniedException denied
                    ? AuthorizationProblems.StatusCode(denied.Problem)
                    : previous?.Invoke(exception) ?? StatusCodes.Status500InternalServerError;
            }
        );
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, AuthorizationDeniedProblemDetailsWriter>());
        services.Configure<MvcOptions>(options => options.Filters.Insert(0, new AuthorizationDeniedExceptionFilter()));

        services.ConfigureAll<OpenApiOptions>(
            options =>
            {
                options.AddOperationTransformer<RequestPoliciesOperationTransformer>();
                options.AddDocumentTransformer<RequestPoliciesSecuritySchemeTransformer>();
            }
        );
    }
}
