using JetBrains.Annotations;
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
        public Assembly? EntryAssembly { get; set; } = null!;
    }
}