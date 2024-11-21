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
public class OptionsConvention : IServiceConvention
{
    private readonly MethodInfo _configureMethod;

    /// <summary>
    ///     A convention that registers any options POCOs that are found with the <see cref="RegisterOptionsConfigurationAttribute" />
    /// </summary>
    public OptionsConvention()
    {
        _configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(
            nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
            [typeof(IServiceCollection), typeof(string), typeof(IConfiguration),]
        )!;
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
            _configureMethod.MakeGenericMethod(options).Invoke(null, [services, attribute.OptionsName, configuration.GetSection(attribute.ConfigurationKey),]);
        }
    }
}
