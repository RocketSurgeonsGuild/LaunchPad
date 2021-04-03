using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;
using System;

[assembly: Convention(typeof(NewtonsoftJsonConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
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
            services
               .AddOptions<MvcNewtonsoftJsonOptions>()
               .Configure<IServiceProvider>(
                    (options, provider) => ActivatorUtilities
                       .CreateInstance<ExistingValueOptionsFactory<JsonSerializerSettings>>(provider, options.SerializerSettings)
                       .Create(nameof(MvcNewtonsoftJsonOptions))
                );
            services
               .Configure<MvcNewtonsoftJsonOptions>(
                    options => options.SerializerSettings.Converters.Add(
                        new ValidationProblemDetailsNewtonsoftJsonConverter()
                    )
                );
        }
    }
}