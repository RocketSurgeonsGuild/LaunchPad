using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.AspNetCore.FluentValidation.NewtonsoftJson
{
    [PublicAPI]
    public static class AspNetCoreFluentValidationNewtonsoftJsonExtensions
    {
        public static IMvcCoreBuilder AddValidationProblemDetailsNewtonsoftJson(
            this IMvcCoreBuilder builder
        )
        {
            AddValidationProblemDetailsNewtonsoftJson(builder.Services);
            return builder;
        }

        public static IMvcBuilder AddValidationProblemDetailsNewtonsoftJson(
            this IMvcBuilder builder
        )
        {
            AddValidationProblemDetailsNewtonsoftJson(builder.Services);
            return builder;
        }

        public static IServiceCollection AddValidationProblemDetailsNewtonsoftJson(
            this IServiceCollection services
        )
        {
            services
               .Configure<MvcNewtonsoftJsonOptions>(
                    options => options.SerializerSettings.Converters.Add(
                        new ValidationProblemDetailsNewtonsoftJsonConverter()
                    )
                );

            return services;
        }
    }
}