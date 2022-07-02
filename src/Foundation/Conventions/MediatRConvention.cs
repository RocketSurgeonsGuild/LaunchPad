using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

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
        services.AddMediatR(
            c =>
            {
                switch (_options)
                {
                    case { MediatorLifetime: ServiceLifetime.Singleton }:
                        c.AsSingleton();
                        break;
                    case { MediatorLifetime: ServiceLifetime.Scoped }:
                        c.AsScoped();
                        break;
                    case { MediatorLifetime: ServiceLifetime.Transient }:
                        c.AsTransient();
                        break;
                }
            },
            context.AssemblyCandidateFinder
                   .GetCandidateAssemblies(nameof(MediatR))
                   .ToArray()
        );
    }
}
