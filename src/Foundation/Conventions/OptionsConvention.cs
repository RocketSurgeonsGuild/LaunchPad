using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     A convention that registers any options POCOs that are found with the <see cref="RegisterOptionsConfigurationAttribute" />
/// </summary>
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)]
public class OptionsConvention : IServiceConvention
{
    private readonly MethodInfo _configureMethod;

    /// <summary>
    ///     A convention that registers any options POCOs that are found with the <see cref="RegisterOptionsConfigurationAttribute" />
    /// </summary>
    public OptionsConvention()
    {
        _configureMethod = GetType().GetMethod(nameof(Configure), BindingFlags.NonPublic | BindingFlags.Static)!;
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var classes = context.TypeProvider.GetTypes(
            s => s.FromAssemblyDependenciesOf<RegisterOptionsConfigurationAttribute>().GetTypes(f => f.WithAttribute<RegisterOptionsConfigurationAttribute>())
        );
        foreach (var options in classes)
        {
            var attribute = options.GetCustomAttribute<RegisterOptionsConfigurationAttribute>()!;
            #pragma warning disable IL2060
            _configureMethod.MakeGenericMethod(options).Invoke(null, [services, attribute.OptionsName, configuration.GetSection(attribute.ConfigurationKey),]);
            #pragma warning restore IL2060
        }
    }

    [RequiresUnreferencedCode("Calls Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(String, IConfiguration)")]
    private static IServiceCollection Configure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(IServiceCollection services, string? name, IConfiguration config)
        where TOptions : class
    {
        return services.Configure<TOptions>(name, config);
    }
}
