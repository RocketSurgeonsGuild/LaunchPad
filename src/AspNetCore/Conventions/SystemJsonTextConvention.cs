using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using NodaTime.Utility;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.Generic;
using System.Linq;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

[assembly: Convention(typeof(SystemJsonTextConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    [AfterConvention(typeof(AspNetCoreConvention))]
    public class SystemJsonTextConvention : IServiceConvention
    {
        private readonly FoundationOptions _options;

        /// <summary>
        /// Create a new SystemJsonTextConvention
        /// </summary>
        /// <param name="options"></param>
        public SystemJsonTextConvention(FoundationOptions? options = null) => _options = options ?? new();

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services
               .AddOptions<JsonOptions>()
               .Configure<IServiceProvider>(
                    (options, provider) => ActivatorUtilities
                       .CreateInstance<ExistingValueOptionsFactory<JsonSerializerOptions>>(provider, options.JsonSerializerOptions)
                       .Create(nameof(JsonOptions))
                );
        }
    }
}