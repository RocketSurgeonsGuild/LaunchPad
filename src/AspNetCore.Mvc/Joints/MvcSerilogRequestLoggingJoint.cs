using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Filters;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

/// <summary>
///     Adds the MVC and Razor Pages filters that add the action and the page handler to the request's log event, in the diagnostic context
///     <see cref="SerilogRequestLoggingJoint" /> registers.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<SerilogRequestLoggingJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class MvcSerilogRequestLoggingJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.Configure<MvcOptions>(
            options =>
            {
                options.Filters.Add<SerilogLoggingActionFilter>(0);
                options.Filters.Add<SerilogLoggingPageFilter>(0);
            }
        );
}
