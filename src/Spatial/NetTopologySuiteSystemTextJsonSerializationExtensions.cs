using System.Text.Json;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     Extensions for noda time
/// </summary>
[PublicAPI]
public static class NetTopologySuiteSystemTextJsonSerializationExtensions
{
    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    [Obsolete("Use ConfigureGeoJsonForLaunchPad instead")]
    public static JsonSerializerOptions ConfigureNetTopologySuiteForLaunchPad(this JsonSerializerOptions options, GeometryFactory? factory)
    {
        return ConfigureGeoJsonForLaunchPad(options, factory);
    }

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
        foreach (var converter in existingConverters) options.Converters.Remove(converter);
        options.Converters.Add(new GeoJsonConverterFactory(factory));
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
        foreach (var converter in existingConverters) options.Converters.Remove(converter);
        options.Converters.Add(new WktConverterFactory(factory));
        return options;
    }
}
