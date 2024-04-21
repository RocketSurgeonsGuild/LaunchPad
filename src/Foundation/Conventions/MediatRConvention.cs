using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class MediatRConvention : IServiceConvention
{
    private readonly FoundationOptions _options;

    /// <summary>
    ///     Create the MediatR convention
    /// </summary>
    /// <param name="options"></param>
    public MediatRConvention(FoundationOptions? options = null)
    {
        _options = options ?? new FoundationOptions();
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        // TODO: Look at converting this to use the assembly type provider
        var assemblies = context.AssemblyProvider.GetAssemblies(x => x.FromAssemblyDependenciesOf<IMediator>()).ToArray();
        if (!assemblies.Any()) return;

        services.AddMediatR(
            c =>
            {
                c.RegisterServicesFromAssemblies(assemblies);
                c.Lifetime = _options switch
                             {
                                 { MediatorLifetime: ServiceLifetime.Singleton, } => ServiceLifetime.Singleton,
                                 { MediatorLifetime: ServiceLifetime.Scoped, }    => ServiceLifetime.Scoped,
                                 { MediatorLifetime: ServiceLifetime.Transient, } => ServiceLifetime.Transient,
                                 _                                                => c.Lifetime,
                             };
            }
        );
    }
}
