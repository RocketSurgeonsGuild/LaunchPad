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
///         The generated <c>Imports</c> seed the application's Specular provider into the context.
///     </para>
///     <para>
///         Dovetail resolves the <see cref="FoundationOptions" /> from the joint context when it creates the joint, and they default when none
///         were set.
///     </para>
/// </remarks>
/// <param name="options">The options, or <see langword="null" /> for the defaults.</param>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class MediatRJoint(FoundationOptions? options = null) : IServiceJoint
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

        var assemblies = context.Require<ISpecularProvider>().GetAssemblies(x => x.DependenciesFromAssemblyOf<IMediator>()).ToArray();
        if (!assemblies.Any()) throw new ArgumentException("No assemblies found that reference MediatR");

        services.AddMediatR(
            c =>
            {
                c.RegisterServicesFromAssemblies(assemblies);
                c.Lifetime = _options switch
                {
                    { MediatorLifetime: ServiceLifetime.Singleton } => ServiceLifetime.Singleton,
                    { MediatorLifetime: ServiceLifetime.Scoped } => ServiceLifetime.Scoped,
                    { MediatorLifetime: ServiceLifetime.Transient } => ServiceLifetime.Transient,
                    _ => c.Lifetime,
                };
            }
        );
    }

    private readonly FoundationOptions _options = options ?? new FoundationOptions();
}
