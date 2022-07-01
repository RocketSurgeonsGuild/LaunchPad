using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class NewtonsoftJsonConvention : IServiceConvention
{
    private readonly FoundationOptions _options;

    /// <summary>
    ///     Creates a convention for newtonsoft json
    /// </summary>
    /// <param name="options"></param>
    public NewtonsoftJsonConvention(FoundationOptions? options = null)
    {
        _options = options ?? new FoundationOptions();
    }

    /// <summary>
    ///     Registers the specified context.
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
