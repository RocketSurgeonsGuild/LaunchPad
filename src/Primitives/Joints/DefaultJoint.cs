using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

/// <summary>
///     The joint every LaunchPad application starts from.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         Adds the options and logging services, then registers every class marked with Specular's service registration
///         attributes using the <see cref="ISpecularProvider" /> found in the joint context.
///     </para>
///     <para>
///         The provider is not created here. Until Dovetail seeds it for the application, the application seeds it itself with
///         <c>builder.Set&lt;ISpecularProvider&gt;(SpecularProvider.Instance)</c>.
///     </para>
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class DefaultJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The joint context.</param>
    /// <param name="services">The service collection.</param>
    /// <exception cref="KeyNotFoundException">No <see cref="ISpecularProvider" /> was set in <paramref name="context" />.</exception>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        services
           .AddOptions()
           .AddLogging();

        services.AddSpecularServiceRegistrations(context.Require<ISpecularProvider>());
    }
}
