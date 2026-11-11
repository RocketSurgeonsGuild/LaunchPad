using Google.Protobuf.WellKnownTypes;

using NodaTime;

using Riok.Mapperly.Abstractions;
using Duration = NodaTime.Duration;
using NodaTimeMapper = Sample.Core.Models.NodaTimeMapper;
using WktDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace Sample.Grpc;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public partial class WellKnownGrpcTypesMapper
{
    public static OffsetDateTime ConvertToOffsetDateTime(Timestamp ts) => OffsetDateTime.FromDateTimeOffset(ts.ToDateTimeOffset());

    public static Timestamp ConvertToTimestamp(OffsetDateTime ts) => Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset());

    public static OffsetDateTime? ConvertToNullableOffsetDateTime(NullableTimestamp ts) => ts.KindCase == NullableTimestamp.KindOneofCase.Data
        ? OffsetDateTime.FromDateTimeOffset(ts.Data.ToDateTimeOffset())
        : default;

    public static NullableTimestamp ConvertFromNullableOffsetDateTime(OffsetDateTime? ts) => ts.HasValue
        ? new() { Data = Timestamp.FromDateTimeOffset(ts.Value.ToDateTimeOffset()) }
        : new() { Null = NullValue.NullValue };

    public static NullableTimestamp ConvertFromOffsetDateTime(OffsetDateTime ts) => new() { Data = Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset()) };

    public static OffsetDateTime ConvertFromOffsetDateTime(NullableTimestamp ts) => OffsetDateTime.FromDateTimeOffset(ts.Data.ToDateTimeOffset());

    public static Duration ConvertToDuration(WktDuration ts) => Duration.FromTimeSpan(ts.ToTimeSpan());

    public static WktDuration ConvertToWktDuration(Duration ts) => WktDuration.FromTimeSpan(ts.ToTimeSpan());

    public static TimeSpan ConvertToTimeSpan(WktDuration ts) => ts.ToTimeSpan();

    public static WktDuration ConvertToWktDuration(TimeSpan ts) => WktDuration.FromTimeSpan(ts);

    // An id that is missing or malformed maps to the empty id, which the validators of the operations reject as invalid.
    public static Core.Models.RocketId MapRocketId(string id) => Guid.TryParse(id, out var value) ? new(value) : default;

    public static Core.Models.LaunchRecordId MapLaunchRecordId(string id) => Guid.TryParse(id, out var value) ? new(value) : default;

    public static partial RocketType MapRocketType(Core.Domain.RocketType request);
    public static partial Core.Domain.RocketType MapRocketType(RocketType request);

    public static NullableRocketType MapNullableRocketType(Core.Domain.RocketType request) => new() { Data = (RocketType)request };

    public static Core.Domain.RocketType MapNullableRocketType(NullableRocketType request) => (Core.Domain.RocketType)request.Data;

    public static NullableRocketType MapRocketType(Core.Domain.RocketType? request) => request.HasValue ? new() { Data = (RocketType)request.Value } : new() { Null = NullValue.NullValue };

    public static Core.Domain.RocketType? MapRocketType(NullableRocketType request) => request.KindCase == NullableRocketType.KindOneofCase.Data
        ? (Core.Domain.RocketType)request.Data
        : default;
}
