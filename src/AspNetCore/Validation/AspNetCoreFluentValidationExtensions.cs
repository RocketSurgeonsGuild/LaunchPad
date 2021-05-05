using FluentValidation;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
    /// <summary>
    /// Extensions for configuring fluent validation based Launch Pad defaults
    /// </summary>
    [PublicAPI]
    public static class AspNetCoreFluentValidationExtensions
    {
        /// <summary>
        /// Adds the extensions with the given configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validatorConfiguration"></param>
        /// <param name="validationMvcConfiguration"></param>
        /// <returns></returns>
        public static IMvcCoreBuilder AddFluentValidationExtensions(
            this IMvcCoreBuilder builder,
            ValidatorConfiguration? validatorConfiguration = null,
            FluentValidationMvcConfiguration? validationMvcConfiguration = null
        )
        {
            AddFluentValidationExtensions(builder.Services, validatorConfiguration, validationMvcConfiguration);
            return builder;
        }

        /// <summary>
        /// Adds the extensions with the given configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validatorConfiguration"></param>
        /// <param name="validationMvcConfiguration"></param>
        /// <returns></returns>
        public static IMvcBuilder AddFluentValidationExtensions(
            this IMvcBuilder builder,
            ValidatorConfiguration? validatorConfiguration = null,
            FluentValidationMvcConfiguration? validationMvcConfiguration = null
        )
        {
            AddFluentValidationExtensions(builder.Services, validatorConfiguration, validationMvcConfiguration);
            return builder;
        }

        /// <summary>
        /// Adds the extensions with the given configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="validatorConfiguration"></param>
        /// <param name="validationMvcConfiguration"></param>
        /// <returns></returns>
        public static IServiceCollection AddFluentValidationExtensions(
            this IServiceCollection services,
            ValidatorConfiguration? validatorConfiguration = null,
            FluentValidationMvcConfiguration? validationMvcConfiguration = null
        )
        {
            validatorConfiguration ??= ValidatorOptions.Global;
            validationMvcConfiguration ??= new FluentValidationMvcConfiguration(validatorConfiguration);
            services
               .Configure<MvcOptions>(options => options.Filters.Insert(0, new ValidationExceptionFilter()))
               .Configure<JsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new ValidationProblemDetailsConverter()));

            services.WithMvcCore().AddFluentValidation(
                config =>
                {
                    foreach (var field in typeof(FluentValidationMvcConfiguration).GetFields(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    ))
                    {
                        field.SetValue(config, field.GetValue(validationMvcConfiguration));
                    }

                    config.ValidatorFactoryType ??= typeof(ValidatorFactory);
                }
            );

            services.AddSingleton<IValidatorInterceptor, ValidatorInterceptor>();
            services.AddSingleton<ProblemDetailsFactory, FluentValidationProblemDetailsFactory>();
            services.PostConfigure<ApiBehaviorOptions>(
                o =>
                {
                    ProblemDetailsFactory? problemDetailsFactory = null;
                    o.InvalidModelStateResponseFactory = context =>
                    {
                        // ProblemDetailsFactory depends on the ApiBehaviorOptions instance. We intentionally avoid constructor injecting
                        // it in this options setup to to avoid a DI cycle.
                        problemDetailsFactory ??= context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        return problemDetailsInvalidModelStateResponse(problemDetailsFactory, context);
                    };

                    static IActionResult problemDetailsInvalidModelStateResponse(ProblemDetailsFactory problemDetailsFactory, ActionContext context)
                    {
                        var problemDetails =
                            problemDetailsFactory.CreateValidationProblemDetails(
                                context.HttpContext,
                                context.ModelState
                            );
                        ObjectResult result;
                        if (problemDetails.Status == 400)
                        {
                            // For compatibility with 2.x, continue producing BadRequestObjectResult instances if the status code is 400.
                            result = new BadRequestObjectResult(problemDetails);
                        }
                        else
                        {
                            result = new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
                        }

                        result.ContentTypes.Add("application/problem+json");
                        result.ContentTypes.Add("application/problem+xml");

                        return result;
                    }
                }
            );

            return services;
        }
    }
}