using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: Convention(typeof(NewtonsoftJsonConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class NewtonsoftJsonConvention : IServiceConvention
    {
        private readonly FoundationOptions _options;

        public NewtonsoftJsonConvention(FoundationOptions? options = null)
        {
            _options = options ?? new();
        }
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

            services.WithMvcCore().AddNewtonsoftJson();
            services.Configure<MvcNewtonsoftJsonOptions>(
                options =>
                {
                    options.SerializerSettings.ConfigureForLaunchPad(_options.DateTimeZoneProvider);
                }
            );
        }
    }
}