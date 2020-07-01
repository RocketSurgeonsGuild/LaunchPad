using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.XPath;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Rocket.Surgery.AspNetCore;
using Rocket.Surgery.Conventions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime.Serialization.JsonNet;
using Rocket.Surgery.Conventions.DependencyInjection;

[assembly: Convention(typeof(NewtonsoftJsonConvention))]

namespace Rocket.Surgery.AspNetCore
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
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IServiceConventionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Services
                .AddMvcCore()
                .AddNewtonsoftJson();
            context.Services.Configure<MvcNewtonsoftJsonOptions>(
                options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                }
            );
        }
    }
}