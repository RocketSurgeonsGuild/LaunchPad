using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that registers MediatR and every handler found in the assemblies that depend on it.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         The assemblies are found with the <see cref="ISpecularProvider" /> in the joint context.
///         The application seeds it with <c>builder.Set&lt;ISpecularProvider&gt;(SpecularProvider.Instance)</c> until Dovetail does.
///     </para>
///     <para>
///         The <see cref="FoundationOptions" /> are read from the joint context when the joint registers, and default when none were set.
///         This stays out of the constructor until Dovetail resolves constructor parameters of exported joints from the context.
///     </para>
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class MediatRJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The joint context.</param>
    /// <param name="services">The service collection.</param>
    /// <exception cref="KeyNotFoundException">No <see cref="ISpecularProvider" /> was set in <paramref name="context" />.</exception>
    /// <exception cref="ArgumentException">No assemblies that reference MediatR were found.</exception>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        var options = context.Get<FoundationOptions>() ?? new FoundationOptions();
        var assemblies = context.Require<ISpecularProvider>().GetAssemblies(x => x.DependenciesFromAssemblyOf<IMediator>()).ToArray();
        if (!assemblies.Any()) throw new ArgumentException("No assemblies found that reference MediatR");

        services.AddMediatR(
            c =>
            {
                c.RegisterServicesFromAssemblies(assemblies);
                c.Lifetime = options switch
                {
                    { MediatorLifetime: ServiceLifetime.Singleton } => ServiceLifetime.Singleton,
                    { MediatorLifetime: ServiceLifetime.Scoped } => ServiceLifetime.Scoped,
                    { MediatorLifetime: ServiceLifetime.Transient } => ServiceLifetime.Transient,
                    _ => c.Lifetime,
                };
            }
        );
    }
}
