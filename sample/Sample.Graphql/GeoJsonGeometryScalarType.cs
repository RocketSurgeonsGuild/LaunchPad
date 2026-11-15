using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using HotChocolate.Execution.Configuration;
using HotChocolate.Features;
using HotChocolate.Language;
using HotChocolate.Text.Json;
using HotChocolate.Types.Spatial;
using HotChocolate.Types.Spatial.Configuration;

using NetTopologySuite.Geometries;

using Rocket.Surgery.LaunchPad.Spatial;

namespace Sample.Graphql;

/// <summary>
///     The <c>Geometry</c> scalar as GeoJSON, read and written by LaunchPad's System.Text.Json GeoJSON converters
/// </summary>
/// <remarks>
///     HotChocolate.Types.Spatial's own <c>GeometryType</c> has a serializer for each simple geometry and the multi geometries but none for a
///     <see cref="GeometryCollection" />, so it rejects one on input and output. This scalar covers every geometry the GeoJSON converters do, and
///     goes through the same converters LaunchPad's StrawberryShake client serializes the scalar with. The JSON itself is read and written through
///     HotChocolate's <see cref="AnyType" />.
/// </remarks>
public sealed class GeoJsonGeometryScalarType : ScalarType<Geometry, ObjectValueNode>
{
    private static readonly JsonTypeInfo<Geometry> GeometryTypeInfo = CreateTypeInfo();

    private static JsonTypeInfo<Geometry> CreateTypeInfo()
    {
        var options = new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null);
        options.MakeReadOnly();
#if NET11_0_OR_GREATER
        return options.GetTypeInfo<Geometry>();
#else
        return (JsonTypeInfo<Geometry>)options.GetTypeInfo(typeof(Geometry));
#endif
    }

    private readonly AnyType _json = new();

    public GeoJsonGeometryScalarType() : base("Geometry", BindingBehavior.Implicit) => Description = "A geometry, as a GeoJSON object";

    protected override Geometry OnCoerceInputLiteral(ObjectValueNode valueLiteral) => Read((JsonElement)_json.CoerceInputLiteral(valueLiteral)!);

    protected override Geometry OnCoerceInputValue(JsonElement inputValue, IFeatureProvider context) => Read(inputValue);

    protected override void OnCoerceOutputValue(Geometry runtimeValue, ResultElement resultValue) =>
        _json.CoerceOutputValue(JsonSerializer.SerializeToElement(runtimeValue, GeometryTypeInfo), resultValue);

    protected override ObjectValueNode OnValueToLiteral(Geometry runtimeValue) =>
        (ObjectValueNode)_json.ValueToLiteral(JsonSerializer.SerializeToElement(runtimeValue, GeometryTypeInfo));

    private static Geometry Read(JsonElement value) =>
        value.Deserialize(GeometryTypeInfo) ?? throw new JsonException("A Geometry cannot be null");
}

internal static class GeoJsonSpatialRequestExecutorBuilderExtensions
{
    /// <summary>
    ///     What HotChocolate's <c>AddSpatialTypes()</c> registers, with <see cref="GeoJsonGeometryScalarType" /> as the <c>Geometry</c> scalar in
    ///     place of its <see cref="GeometryType" />: <c>AddSpatialTypes()</c> always adds that one, and two types cannot share the name.
    /// </summary>
    /// <remarks>
    ///     The one thing left out is HotChocolate's internal geometry transformer interceptor, which reprojects geometries between the coordinate
    ///     systems registered on the spatial convention; the sample registers none.
    /// </remarks>
    public static IRequestExecutorBuilder AddGeoJsonSpatialTypes(this IRequestExecutorBuilder builder) =>
        builder
           .TryAddConvention<ISpatialConvention>(_ => new SpatialConvention())
           .AddType<GeoJsonInterfaceType>()
           .AddType<GeoJsonPointInputType>()
           .AddType<GeoJsonMultiPointInputType>()
           .AddType<GeoJsonLineStringInputType>()
           .AddType<GeoJsonMultiLineStringInputType>()
           .AddType<GeoJsonPolygonInputType>()
           .AddType<GeoJsonMultiPolygonInputType>()
           .AddType<GeoJsonPointType>()
           .AddType<GeoJsonMultiPointType>()
           .AddType<GeoJsonLineStringType>()
           .AddType<GeoJsonMultiLineStringType>()
           .AddType<GeoJsonPolygonType>()
           .AddType<GeoJsonMultiPolygonType>()
           .AddType<GeoJsonGeometryEnumType>()
           .AddType<GeoJsonGeometryScalarType>()
           .BindRuntimeType<Geometry, GeoJsonGeometryScalarType>()
           .BindRuntimeType<Coordinate, GeoJsonPositionType>();
}
