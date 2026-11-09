using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     Extensions for NetTopologySuite
/// </summary>
/// <remarks>
///     Both methods also chain <see cref="TypeInfoResolver" /> into the options' <see cref="JsonSerializerOptions.TypeInfoResolverChain" />, so the
///     geometry and feature types resolve in a trimmed or native AOT application. No reflection based resolver is added: options
///     that should reflect over other types need one of their own.
/// </remarks>
[PublicAPI]
public static class NetTopologySuiteSystemTextJsonSerializationExtensions
{
    /// <summary>
    ///     Resolves the metadata of the geometry and feature types from the converter factory in the options, without reflection, for when they are
    ///     serialized at the root or as an <see cref="object" />.
    /// </summary>
    public static IJsonTypeInfoResolver TypeInfoResolver => SpatialJsonTypeInfoResolver.Instance;

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static JsonSerializerOptions ConfigureGeoJsonForLaunchPad(this JsonSerializerOptions options, GeometryFactory? factory)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        var existingConverters = options.Converters.OfType<GeoJsonConverterFactory>().ToArray();
        foreach (var converter in existingConverters)
        {
            options.Converters.Remove(converter);
        }

        options.Converters.Add(new GeoJsonConverterFactory(factory));
        SpatialJsonTypeInfoResolver.AddTo(options);
        return options;
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static JsonSerializerOptions ConfigureWellKnownTextForLaunchPad(this JsonSerializerOptions options, GeometryFactory? factory)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        var existingConverters = options.Converters.OfType<GeoJsonConverterFactory>().ToArray();
        foreach (var converter in existingConverters)
        {
            options.Converters.Remove(converter);
        }

        options.Converters.Add(new WktConverterFactory(factory));
        SpatialJsonTypeInfoResolver.AddTo(options);
        return options;
    }
}
