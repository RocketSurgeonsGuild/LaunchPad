using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     A convention that registers any options POCOs that are found with the <see cref="RegisterOptionsConfigurationAttribute" />
/// </summary>
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)]
public class OptionsConvention : IServiceConvention
{
    [RequiresUnreferencedCode(
        "Calls Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(String, IConfiguration)"
    )]
    private static IServiceCollection Configure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(
        IServiceCollection services,
        string? name,
        IConfiguration config
    )
        where TOptions : class =>
        services.Configure<TOptions>(name, config);

    private readonly MethodInfo _configureMethod;

    /// <summary>
    ///     A convention that registers any options POCOs that are found with the <see cref="RegisterOptionsConfigurationAttribute" />
    /// </summary>
    public OptionsConvention() => _configureMethod = GetType().GetMethod(nameof(Configure), BindingFlags.NonPublic | BindingFlags.Static)!;

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var classes = context
                     .Assembly.GetCompiledTypeProvider()
                     .GetTypes(
                          s => s
                              .FromAssemblies()
                              .GetTypes(
                                   z => z
                                       .NotInfoOf(TypeInfoFilter.Abstract, TypeInfoFilter.Static, TypeInfoFilter.GenericType)
                                       .KindOf(TypeKindFilter.Class)
                                       .WithAttribute<RegisterOptionsConfigurationAttribute>()
                               )
                      );

        foreach ((var options, var attribute) in classes.SelectMany(z => z.GetCustomAttributes<RegisterOptionsConfigurationAttribute>(), (type, attribute) => (type, attribute)))
        {
            _ = _configureMethod
               .MakeGenericMethod(options)
               .Invoke(null, [services, attribute.OptionsName, configuration.GetSection(attribute.ConfigurationKey)]);
        }
    }
}
