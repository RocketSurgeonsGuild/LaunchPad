using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Create the MediatR convention
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]

public class MediatRConvention(FoundationOptions? options = null) : IServiceConvention
{
    private readonly FoundationOptions _options = options ?? new FoundationOptions();

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var assemblies = context.Assembly.GetCompiledTypeProvider().GetAssemblies(x => x.FromAssemblyDependenciesOf<IMediator>()).ToArray();
        if (!assemblies.Any())
        {
            throw new ArgumentException("No assemblies found that reference MediatR");
        }

        services.AddMediatR(
            c =>
            {
                c.RegisterServicesFromAssemblies(assemblies);
                c.Lifetime = _options switch
                {
                    { MediatorLifetime: ServiceLifetime.Singleton, } => ServiceLifetime.Singleton,
                    { MediatorLifetime: ServiceLifetime.Scoped, } => ServiceLifetime.Scoped,
                    { MediatorLifetime: ServiceLifetime.Transient, } => ServiceLifetime.Transient,
                    _ => c.Lifetime,
                };
            }
        );
    }
}
