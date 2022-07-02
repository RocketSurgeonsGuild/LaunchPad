using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.Extensions.Configuration;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     Registers json configuration
/// </summary>
[PublicAPI]
[ExportConvention]
public class ConfigurationConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        context
           .GetOrAdd(() => new ConfigOptions())
           .UseJson()
           .UseYml()
           .UseYaml()
            ;
    }
}
