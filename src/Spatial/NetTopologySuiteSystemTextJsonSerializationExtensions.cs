using JetBrains.Annotations;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Text.Json;

namespace Rocket.Surgery.LaunchPad.Spatial
{
    /// <summary>
    /// Extensions for noda time
    /// </summary>
    [PublicAPI]
    public static class NetTopologySuiteSystemTextJsonSerializationExtensions
    {
        /// <summary>
        /// Configure System.Text.Json with defaults for launchpad
        /// </summary>
        /// <param name="options"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static JsonSerializerOptions ConfigureNetTopologySuiteForLaunchPad(this JsonSerializerOptions options, GeometryFactory? factory)
        {

            factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            options.Converters.Add(new GeoJsonConverterFactory(factory));
            return options;
        }
    }
}