using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    /// <summary>
    /// Extensions used to pull in MediatR for launch pad
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadAspNetCoreFluentValidationExtension
    {
        /// <summary>
        /// Adds the launchpad services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="validatorConfiguration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPadAspNetCoreFluentValidation(
            this IServiceCollection services,
            Action<FluentValidationMvcConfiguration>? validatorConfiguration = null
        )
        {
            services.Configure<MvcOptions>(
                options =>
                {
                    options.Filters.Add<NotFoundExceptionFilter>();
                    options.Filters.Add<RequestFailedExceptionFilter>();
                    options.Filters.Add<SerilogLoggingActionFilter>(0);
                    options.Filters.Add<SerilogLoggingPageFilter>(0);
                }
            );

            services.AddFluentValidationExtensions(validatorConfiguration);

            return services;
        }
    }

    /// <summary>
    /// Extensions used to add additional system.text.json configurations
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadAspNetCoreSystemTextJsonExtension
    {
        /// <summary>
        /// Adds the launchpad services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dateTimeZoneProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddLaunchPadSystemJsonText(this IServiceCollection services, IDateTimeZoneProvider? dateTimeZoneProvider = null)
        {
            services.Configure<JsonOptions>(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ConfigureForNodaTime(dateTimeZoneProvider ?? DateTimeZoneProviders.Tzdb);
                }
            );

            return services;
        }
    }
}