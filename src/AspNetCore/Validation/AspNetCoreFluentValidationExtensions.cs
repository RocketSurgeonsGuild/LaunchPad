using System.Reflection;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.SpaceShuttle.Extensions.Validation;

namespace Rocket.Surgery.SpaceShuttle.AspNetCore.Validation
{
    [PublicAPI]
    public static class AspNetCoreFluentValidationExtensions
    {
        public static IMvcCoreBuilder AddFluentValidationExtensions(
            this IMvcCoreBuilder builder,
            FluentValidationMvcConfiguration? configuration = null
        )
        {
            AddFluentValidationExtensions(builder.Services, configuration);
            return builder;
        }

        public static IMvcBuilder AddFluentValidationExtensions(
            this IMvcBuilder builder,
            FluentValidationMvcConfiguration? configuration = null
        )
        {
            AddFluentValidationExtensions(builder.Services, configuration);
            return builder;
        }

        public static IServiceCollection AddFluentValidationExtensions(
            this IServiceCollection services,
            FluentValidationMvcConfiguration? configuration = null
        )
        {
            configuration ??= new FluentValidationMvcConfiguration();
            services
               .Configure<MvcOptions>(
                    options =>
                        options.Filters.Insert(0, new ValidationExceptionFilter())
                )
               .Configure<JsonOptions>(
                    options =>
                        options.JsonSerializerOptions.Converters.Add(new ValidationProblemDetailsConverter())
                );

            services.AddMvcCore().AddFluentValidation(
                config =>
                {
                    foreach (var field in typeof(FluentValidationMvcConfiguration).GetFields(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    ))
                    {
                        field.SetValue(config, field.GetValue(configuration));
                    }

                    config.ValidatorFactoryType ??= typeof(ValidatorFactory);
                }
            );

            services.AddSingleton<IValidatorInterceptor, ValidatorInterceptor>();
            services.AddSingleton<ProblemDetailsFactory, FluentValidationProblemDetailsFactory>();
            services.Configure<ApiBehaviorOptions>(
                o =>
                {
                    ProblemDetailsFactory? problemDetailsFactory = null;
                    o.InvalidModelStateResponseFactory = context =>
                    {
                        // ProblemDetailsFactory depends on the ApiBehaviorOptions instance. We intentionally avoid constructor injecting
                        // it in this options setup to to avoid a DI cycle.
                        problemDetailsFactory ??= context.HttpContext.RequestServices
                           .GetRequiredService<ProblemDetailsFactory>();
                        return problemDetailsInvalidModelStateResponse(problemDetailsFactory, context);
                    };

                    static IActionResult problemDetailsInvalidModelStateResponse(
                        ProblemDetailsFactory problemDetailsFactory,
                        ActionContext context
                    )
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