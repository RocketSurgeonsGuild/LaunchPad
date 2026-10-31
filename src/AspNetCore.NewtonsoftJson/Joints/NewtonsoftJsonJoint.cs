using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Joint that wires Newtonsoft Json into ASP.NET Core MVC.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class NewtonsoftJsonJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services"></param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

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
