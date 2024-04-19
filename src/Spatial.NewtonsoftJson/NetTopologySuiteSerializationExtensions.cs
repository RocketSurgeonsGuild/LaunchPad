using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     Extensions for noda time
/// </summary>
[PublicAPI]
public static class NetTopologySuiteSerializationExtensions
{
    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    [Obsolete("Use ConfigureGeoJsonForLaunchPad instead")]
    public static JsonSerializer ConfigureNetTopologySuiteForLaunchPad(this JsonSerializer options, GeometryFactory? factory, int dimension = 2)
    {
        return ConfigureGeoJsonForLaunchPad(options, factory, dimension);
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public static JsonSerializer ConfigureGeoJsonForLaunchPad(this JsonSerializer options, GeometryFactory? factory, int dimension = 2)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        ApplyGeoJsonConverters(factory, dimension, options.Converters);
        return options;
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public static JsonSerializer ConfigureWellKnownTextForLaunchPad(this JsonSerializer options, GeometryFactory? factory, int dimension = 2)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        ApplyWellKnownTextConverters(factory, dimension, options.Converters);
        return options;
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    [Obsolete("Use ConfigureGeoJsonForLaunchPad instead")]
    public static JsonSerializerSettings ConfigureNetTopologySuiteForLaunchPad(this JsonSerializerSettings options, GeometryFactory? factory, int dimension = 2)
    {
        return ConfigureGeoJsonForLaunchPad(options, factory, dimension);
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public static JsonSerializerSettings ConfigureGeoJsonForLaunchPad(this JsonSerializerSettings options, GeometryFactory? factory, int dimension = 2)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        ApplyGeoJsonConverters(factory, dimension, options.Converters);
        return options;
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public static JsonSerializerSettings ConfigureWellKnownTextForLaunchPad(this JsonSerializerSettings options, GeometryFactory? factory, int dimension = 2)
    {
        factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        ApplyWellKnownTextConverters(factory, dimension, options.Converters);
        return options;
    }

    private static void ApplyConverters(GeometryFactory factory, int dimension, IList<JsonConverter> converters)
    {
        converters.Add(new FeatureCollectionConverter());
        converters.Add(new FeatureConverter());
        converters.Add(new AttributesTableConverter());
        converters.Add(new EnvelopeConverter());
    }

    private static void ApplyGeoJsonConverters(GeometryFactory factory, int dimension, IList<JsonConverter> converters)
    {
        var convertersToRemove = converters
                                .OfType<GeometryConverter>()
                                .Cast<JsonConverter>()
                                .ToList();
        foreach (var converter in convertersToRemove)
        {
            converters.Remove(converter);
        }

        converters.Add(new GeometryConverter(factory, dimension));
        ApplyConverters(factory, dimension, converters);
    }

    private static void ApplyWellKnownTextConverters(GeometryFactory factory, int dimension, IList<JsonConverter> converters)
    {
        var convertersToRemove = converters
                                .OfType<GeometryConverter>()
                                .Cast<JsonConverter>()
                                .ToList();
        foreach (var converter in convertersToRemove)
        {
            converters.Remove(converter);
        }

        converters.Add(new WktGeometryConverter(factory, dimension));
        ApplyConverters(factory, dimension, converters);
    }
}
