using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    internal static class ConfigurationAsyncHelper
    {
        public static bool IsAsync([NotNull] IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration.GetValue("ApplicationState:IsDefaultCommand", true);
        }
    }
}