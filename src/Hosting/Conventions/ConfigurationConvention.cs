using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;

[assembly: Convention(typeof(ConfigurationConvention))]

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions
{
    /// <summary>
    /// Registers json configuration
    /// </summary>
    [PublicAPI]
    public class ConfigurationConvention : IHostingConvention
    {
        /// <inheritdoc />
        public void Register(IHostingConventionContext context)
        {
            context.GetOrAdd(() => new ConfigOptions())
               .UseJson()
               .UseYml()
               .UseYaml();
        }
    }
}