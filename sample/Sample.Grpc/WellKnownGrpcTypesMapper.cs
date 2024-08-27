using Google.Protobuf.WellKnownTypes;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Duration = NodaTime.Duration;
using WktDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace Sample.Grpc;

[PublicAPI, Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public partial class WellKnownGrpcTypesMapper
{
    public static Instant ConvertToInstant(Timestamp ts) => Instant.FromDateTimeOffset(ts.ToDateTimeOffset());
    public static Timestamp ConvertToTimestamp(Instant ts) => Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset());

    public static Instant? ConvertToNullableInstant(NullableTimestamp ts) =>
        ts.KindCase == NullableTimestamp.KindOneofCase.Data
            ? Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset())
            : default;

    public static NullableTimestamp ConvertFromNullableInstant(Instant? ts) => ts.HasValue
        ? new() { Data = Timestamp.FromDateTimeOffset(ts.Value.ToDateTimeOffset()) }
        : new() { Null = NullValue.NullValue };

    public static NullableTimestamp ConvertFromInstant(Instant ts) => new() { Data = Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset()) };
    public static Instant ConvertFromInstant(NullableTimestamp ts) => Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset());

    public static Duration ConvertToDuration(WktDuration ts) => Duration.FromTimeSpan(ts.ToTimeSpan());
    public static WktDuration ConvertToWktDuration(Duration ts) => WktDuration.FromTimeSpan(ts.ToTimeSpan());

    public static TimeSpan ConvertToTimeSpan(WktDuration ts) => ts.ToTimeSpan();
    public static WktDuration ConvertToWktDuration(TimeSpan ts) => WktDuration.FromTimeSpan(ts);

    public static partial RocketType MapRocketType(Core.Domain.RocketType request);
    public static partial Core.Domain.RocketType MapRocketType(RocketType request);

    public static NullableRocketType MapNullableRocketType(Core.Domain.RocketType request) => new() { Data = (RocketType)request };
    public static Core.Domain.RocketType MapNullableRocketType(NullableRocketType request) => (Core.Domain.RocketType)request.Data;

    public static NullableRocketType MapRocketType(Core.Domain.RocketType? request) =>
        request.HasValue ? new() { Data = (RocketType)request.Value } : new() { Null = NullValue.NullValue };

    public static Core.Domain.RocketType? MapRocketType(NullableRocketType request) =>
        request.KindCase == NullableRocketType.KindOneofCase.Data
            ? (Core.Domain.RocketType)request.Data
            : default;
}
