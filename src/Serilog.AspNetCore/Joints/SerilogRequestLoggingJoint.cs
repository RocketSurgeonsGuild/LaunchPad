using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;

using Serilog;
using Serilog.Core;
using Serilog.Extensions.Hosting;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Wires Serilog's request logging into ASP.NET Core: the MVC and Razor Pages filters that add the action and the page handler to the
///     request's log event, and the <see cref="IDiagnosticContext" /> they and <c>UseLaunchPadRequestLogging</c> collect the properties in.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The diagnostic context is only added when none is registered yet, so the one LaunchPad.Hosting writes into an application that references
///     Joinery.Dovetail.Serilog is kept. It writes to the Serilog <see cref="Logger" /> in the container, or to the static logger when there is none.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class SerilogRequestLoggingJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.TryAddSingleton(provider => new DiagnosticContext(provider.GetService<Logger>()));
        services.TryAddSingleton<IDiagnosticContext>(provider => provider.GetRequiredService<DiagnosticContext>());
        services.Configure<MvcOptions>(
            options =>
            {
                options.Filters.Add<SerilogLoggingActionFilter>(0);
                options.Filters.Add<SerilogLoggingPageFilter>(0);
            }
        );
    }
}
