using JetBrains.Annotations;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="factory"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static JsonSerializerSettings ConfigureNetTopologySuiteForLaunchPad(this JsonSerializerSettings options, GeometryFactory? factory, int dimension = 2)
        {
            factory ??= NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var converters = options.Converters;

            converters.Add(new FeatureCollectionConverter());
            converters.Add(new FeatureConverter());
            converters.Add(new AttributesTableConverter());
            converters.Add(new GeometryConverter(factory, dimension));
            converters.Add(new GeometryArrayConverter(factory, dimension));
            converters.Add(new CoordinateConverter(factory.PrecisionModel, dimension));
            converters.Add(new EnvelopeConverter());
            return options;
        }
    }
}