using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     Joint for working with Newtonsoft Json
/// </summary>
/// <remarks>
///     A service Joint that configures the Newtonsoft Json serializer settings.
///     The NodaTime converters are added by the NodaTime.NewtonsoftJson package, and the Newtonsoft Json types are destructured in Serilog
///     by the NewtonsoftJson.Serilog package.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NewtonsoftJsonJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<JsonSerializerSettings>>(
            _ =>
                new ConfigureNamedOptions<JsonSerializerSettings>(
                    null,
                    options => options.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()))
                )
        );
    }
}
