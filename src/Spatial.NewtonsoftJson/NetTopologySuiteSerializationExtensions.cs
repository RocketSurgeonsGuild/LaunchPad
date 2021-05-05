using JetBrains.Annotations;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.Spatial
{
    /// <summary>
    /// Extensions for noda time
    /// </summary>
    [PublicAPI]
    public static class NetTopologySuiteSerializationExtensions
    {
        /// <summary>
        /// Configure System.Text.Json with defaults for launchpad
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dateTimeZoneProvider"></param>
        /// <returns></returns>
        public static JsonSerializer ConfigureNetTopologySuiteForLaunchPad(this JsonSerializer options, GeometryFactory? factory, int dimension = 2)
        {
            factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var converters = options.Converters;
            ApplyConverters(factory, dimension, options.Converters);
            return options;
        }

        /// <summary>
        /// Configure System.Text.Json with defaults for launchpad
        /// </summary>
        /// <param name="options"></param>
        /// <param name="factory"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static JsonSerializerSettings ConfigureNetTopologySuiteForLaunchPad(this JsonSerializerSettings options, GeometryFactory? factory, int dimension = 2)
        {
            factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            ApplyConverters(factory, dimension, options.Converters);
            return options;
        }

        private static void ApplyConverters(GeometryFactory factory, int dimension, IList<JsonConverter> converters)
        {
            converters.Add(new FeatureCollectionConverter());
            converters.Add(new FeatureConverter());
            converters.Add(new AttributesTableConverter());
            converters.Add(new GeometryConverter(factory, dimension));
            converters.Add(new GeometryArrayConverter(factory, dimension));
            converters.Add(new CoordinateConverter(factory.PrecisionModel, dimension));
            converters.Add(new EnvelopeConverter());
        }
    }
}