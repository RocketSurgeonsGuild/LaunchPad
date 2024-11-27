using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(AspNetCoreConvention))]
[ConventionCategory(ConventionCategory.Application)]
public partial class FluentValidationConvention : IServiceConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddFluentValidationClientsideAdapters();
        services
           .Configure<MvcOptions>(mvcOptions => mvcOptions.Filters.Insert(0, new ValidationExceptionFilter()))
           .Configure<MvcJsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new ValidationProblemDetailsConverter ()))
           .Configure<HttpJsonOptions>(options => options.SerializerOptions.Converters.Add(new ValidationProblemDetailsConverter()));

//        AddFluentValidationRules(services);
    }
}
