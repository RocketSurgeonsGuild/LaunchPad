using FluentValidation.AspNetCore;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Wires FluentValidation into ASP.NET Core: the client side adapters, the validation exception filter, the
///     problem details converter, and the <see cref="FluentValidationProblemDetails" /> as the documented validation response of the restful api
///     controllers.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class FluentValidationJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddFluentValidationClientsideAdapters();
        services
           .Configure<MvcOptions>(mvcOptions => mvcOptions.Filters.Insert(0, new ValidationExceptionFilter()))
           .Configure<MvcJsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new ValidationProblemDetailsConverter()))
           .Configure<HttpJsonOptions>(options => options.SerializerOptions.Converters.Add(new ValidationProblemDetailsConverter()))
           .Configure<RestfulApiOptions>(options => options.ValidationProblemDetailsType = typeof(FluentValidationProblemDetails));
    }
}
