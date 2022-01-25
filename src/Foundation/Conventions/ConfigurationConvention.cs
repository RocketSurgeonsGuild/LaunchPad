using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;

[assembly: Convention(typeof(ConfigurationConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    /// <summary>
    ///     Registers json configuration
    /// </summary>
    [PublicAPI]
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
}
