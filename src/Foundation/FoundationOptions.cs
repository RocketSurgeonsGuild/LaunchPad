using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.TimeZones;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    /// <summary>
    /// Common foundation options
    /// </summary>
    [PublicAPI]
    public class FoundationOptions
    {
        /// <summary>
        /// The executing assembly
        /// </summary>
        /// <remarks>
        /// Useful so that applications and conventions can know the "true" executing assembly when running in an environment like azure functions
        /// </remarks>
        public Assembly? EntryAssembly { get; set; }

        /// <summary>
        /// The NodaTime timezone source
        /// </summary>
        public IDateTimeZoneSource DateTimeZoneSource { get; set; } = TzdbDateTimeZoneSource.Default;

        /// <summary>
        /// The Mediator lifetime
        /// </summary>
        public ServiceLifetime MediatorLifetime { get; set; } = ServiceLifetime.Transient;

    }
}