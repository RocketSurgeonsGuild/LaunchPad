using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.Spatial;

// Builds the metadata of the NetTopologySuite types from the converter factory in the options (GeoJSON or WKT), so they need no reflection or
// source generated context. The factories hand out one converter for a whole hierarchy (a JsonConverter<Geometry> for a Point), so a derived type
// gets a converter of its own that forwards to it.
internal sealed class SpatialJsonTypeInfoResolver : IJsonTypeInfoResolver
{
    private static readonly Dictionary<Type, Func<JsonSerializerOptions, JsonTypeInfo?>> Factories = new()
    {
        [typeof(Geometry)] = Create<Geometry, Geometry>,
        [typeof(Point)] = Create<Point, Geometry>,
        [typeof(LineString)] = Create<LineString, Geometry>,
        [typeof(LinearRing)] = Create<LinearRing, Geometry>,
        [typeof(Polygon)] = Create<Polygon, Geometry>,
        [typeof(MultiPoint)] = Create<MultiPoint, Geometry>,
        [typeof(MultiLineString)] = Create<MultiLineString, Geometry>,
        [typeof(MultiPolygon)] = Create<MultiPolygon, Geometry>,
        [typeof(GeometryCollection)] = Create<GeometryCollection, Geometry>,
        [typeof(IFeature)] = Create<IFeature, IFeature>,
        [typeof(Feature)] = Create<Feature, IFeature>,
        [typeof(FeatureCollection)] = Create<FeatureCollection, FeatureCollection>,
        [typeof(IAttributesTable)] = Create<IAttributesTable, IAttributesTable>,
        [typeof(AttributesTable)] = Create<AttributesTable, IAttributesTable>,
    };

    public static IJsonTypeInfoResolver Instance { get; } = new SpatialJsonTypeInfoResolver();

    public static void AddTo(JsonSerializerOptions options)
    {
        var chain = options.TypeInfoResolverChain;
        if (chain.Contains(Instance)) return;
        chain.Add(Instance);
    }

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) => Factories.TryGetValue(type, out var factory) ? factory(options) : null;

    private static JsonTypeInfo? Create<T, TBase>(JsonSerializerOptions options) where T : TBase
    {
        foreach (var converter in options.Converters)
        {
            if (!converter.CanConvert(typeof(T))) continue;
            switch (converter is JsonConverterFactory factory ? factory.CreateConverter(typeof(T), options) : converter)
            {
                case JsonConverter<T> typed:
                    return JsonMetadataServices.CreateValueInfo<T>(options, typed);
                case JsonConverter<TBase> hierarchy:
                    return JsonMetadataServices.CreateValueInfo<T>(options, new DerivedConverter<T, TBase>(hierarchy));
                default:
                    break;
            }
        }

        return null;
    }

    private sealed class DerivedConverter<T, TBase>(JsonConverter<TBase> inner) : JsonConverter<T> where T : TBase
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (T?)inner.Read(ref reader, typeof(TBase), options);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => inner.Write(writer, value, options);
    }
}
