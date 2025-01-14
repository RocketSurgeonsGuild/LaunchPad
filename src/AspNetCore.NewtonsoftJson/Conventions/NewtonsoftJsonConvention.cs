using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Creates a convention for newtonsoft json
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Application)]
public class NewtonsoftJsonConvention(FoundationOptions? options = null) : IServiceConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
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

    private readonly FoundationOptions _options = options ?? new FoundationOptions();
}
