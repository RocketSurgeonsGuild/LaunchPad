using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog;
using Serilog.Core;
using Serilog.Extensions.Hosting;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Wires Serilog's request logging into ASP.NET Core: the <see cref="IDiagnosticContext" /> that <c>UseLaunchPadRequestLogging</c> collects
///     the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the
///     page handler.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The diagnostic context is added with <c>TryAdd</c>, the same as the one LaunchPad.Hosting writes into an application that references
///     Joinery.Dovetail.Serilog, so whichever registers first is used. Both bind the properties through the Serilog <see cref="Logger" /> in the
///     container; this one falls back to the static logger when there is none.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
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
    }
}
