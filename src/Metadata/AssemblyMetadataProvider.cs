using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.Metadata
{
    /// <summary>
    /// An information provider to pull assembly data out of the assembly
    /// </summary>
    public class AssemblyMetadataProvider
    {
        private readonly ILookup<string, string> _results;

        /// <summary>
        /// Create a new assembly metadata provider
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyMetadataProvider Create(Assembly assembly) => new AssemblyMetadataProvider(assembly);

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyMetadataProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyMetadataProvider(Assembly assembly) => _results = assembly
           .GetCustomAttributes<AssemblyMetadataAttribute>()
           .ToLookup(
                x => x.Key,
                x => x.Value,
                StringComparer.OrdinalIgnoreCase
            );

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String[].</returns>
        public IEnumerable<string> GetValue(string key) => _results.Contains(key) ? _results[key] : Array.Empty<string>();

        /// <summary>
        /// Determines whether the specified key has prefix.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key has prefix; otherwise, <c>false</c>.</returns>
        public bool HasPrefix(string key) => _results.Any(z => z.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Get all the available keys
        /// </summary>
        public IEnumerable<string> Keys => _results.Select(z => z.Key).Distinct();

        /// <summary>
        /// The raw metadata results for the assembly
        /// </summary>
        public ILookup<string, string> Data => _results;
    }
}