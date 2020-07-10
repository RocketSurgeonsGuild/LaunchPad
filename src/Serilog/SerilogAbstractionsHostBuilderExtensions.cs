// ReSharper disable once CheckNamespace

using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    [PublicAPI]
    public static class SerilogAbstractionsHostBuilderExtensions
    {
        /// <summary>
        /// Configure the serilog delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureSerilog(
            [NotNull] this IConventionHostBuilder container,
            [NotNull] SerilogConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}