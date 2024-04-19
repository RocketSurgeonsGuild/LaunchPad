using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     AutoMapperConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class AutoMapperConvention : IServiceConvention
{
    private readonly AutoMapperOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AutoMapperConvention" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public AutoMapperConvention(AutoMapperOptions? options = null)
    {
        _options = options ?? new AutoMapperOptions();
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var provider = context.AssemblyProvider;
        var assemblies = provider.GetAssemblies(f => f.FromAssemblyDependenciesOf<IMapper>());
        services.AddAutoMapper(assemblies, _options.ServiceLifetime);
    }
}
