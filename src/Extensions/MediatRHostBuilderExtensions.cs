﻿using System;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Extensions.Conventions;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    /// <summary>
    /// MediatRHostBuilderExtensions.
    /// </summary>
    [PublicAPI]
    public static class LaunchPadHostBuilderExtensions
    {
        /// <summary>
        /// Adds MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseMediatR([NotNull] this IHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.GetConventions().UseMediatR();
            return builder;
        }

        /// <summary>
        /// Adds MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseMediatR([NotNull] this IConventionHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Scanner.PrependConvention<MediatRConvention>();
            return builder;
        }

        /// <summary>
        /// Adds MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceConfig">The MediatR service configuration.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseMediatR(
            this IHostBuilder builder,
            MediatRServiceConfiguration serviceConfig
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (serviceConfig is null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            builder.GetConventions().UseMediatR(serviceConfig);
            return builder;
        }

        /// <summary>
        /// Adds MediatR.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceConfig">The MediatR service configuration.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseMediatR(
            this IConventionHostBuilder builder,
            MediatRServiceConfiguration serviceConfig
        )
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (serviceConfig is null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            builder.Set(serviceConfig);
            builder.Scanner.PrependConvention<MediatRConvention>();
            return builder;
        }

        /// <summary>
        /// Adds fluent validation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseFluentValidation([NotNull] this IHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.GetConventions().Scanner.PrependConvention<FluentValidationConvention>();
            return builder;
        }

        /// <summary>
        /// Adds fluent validation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseFluentValidation([NotNull] this IConventionHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Scanner.PrependConvention<FluentValidationConvention>();
            return builder;
        }
    }
}