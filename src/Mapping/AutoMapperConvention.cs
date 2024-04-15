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
        var profiles = context
                      .AssemblyProvider.GetTypes(t => t.FromAssemblyDependenciesOf<Mapper>().GetTypes(f => f.AssignableTo<Profile>().NotInfoOf(TypeInfoFilter.Abstract))).ToArray();

        var autoMapperTypes = context.AssemblyProvider.GetTypes(
            t => t
                .FromAssemblyDependenciesOf<Mapper>()
                .GetTypes(
                     f => f
                         .AssignableToAny(
                              typeof(IValueResolver<,,>),
                              typeof(IMemberValueResolver<,,,>),
                              typeof(ITypeConverter<,>),
                              typeof(IValueConverter<,>),
                              typeof(IMappingAction<,>)
                          )
                         .NotInfoOf(TypeInfoFilter.Abstract)
                         .KindOf(TypeKindFilter.Class)
                 )
        );
        foreach (var type in autoMapperTypes)
        {
            services.TryAdd(new ServiceDescriptor(type, type, _options.ServiceLifetime));
        }

        services.AddAutoMapper(
            config =>
            {
                foreach (var profile in profiles)
                {
                    config.AddProfile(profile);
                }
            }
        );
    }
}
