using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.AspNetCore.Composition;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers the controllers, and the application model provider that gives them the restful api conventions.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class RestfulJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services
           .AddControllers()
           .AddControllersAsServices();

        services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, RestfulApiApplicationModelProvider>());
    }
}
