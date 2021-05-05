using JetBrains.Annotations;
using Rocket.Surgery.LaunchPad.Mapping;
using System;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// AutoMapperHostBuilderExtensions.
    /// </summary>
    [PublicAPI]
    public static class AutoMapperHostBuilderExtensions
    {
        /// <summary>
        /// Uses AutoMapper.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="options">The options object</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static ConventionContextBuilder UseAutoMapper([NotNull] this ConventionContextBuilder container, AutoMapperOptions? options = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.PrependConvention<AutoMapperConvention>();
            return container;
        }
    }
}