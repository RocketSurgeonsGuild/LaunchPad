using Google.Protobuf.WellKnownTypes;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Duration = NodaTime.Duration;
using WktDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace Sample.Grpc;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public partial class WellKnownGrpcTypesMapper
{
    public static Instant ConvertToInstant(Timestamp ts)
    {
        return Instant.FromDateTimeOffset(ts.ToDateTimeOffset());
    }

    public static Timestamp ConvertToTimestamp(Instant ts)
    {
        return Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset());
    }

    public static Instant? ConvertToNullableInstant(NullableTimestamp ts)
    {
        return ts.KindCase == NullableTimestamp.KindOneofCase.Data
            ? Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset())
            : default;
    }

    public static NullableTimestamp ConvertFromNullableInstant(Instant? ts)
    {
        return ts.HasValue
            ? new() { Data = Timestamp.FromDateTimeOffset(ts.Value.ToDateTimeOffset()), }
            : new() { Null = NullValue.NullValue, };
    }

    public static NullableTimestamp ConvertFromInstant(Instant ts)
    {
        return new() { Data = Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset()), };
    }

    public static Instant ConvertFromInstant(NullableTimestamp ts)
    {
        return Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset());
    }

    public static Duration ConvertToDuration(WktDuration ts)
    {
        return Duration.FromTimeSpan(ts.ToTimeSpan());
    }

    public static WktDuration ConvertToWktDuration(Duration ts)
    {
        return WktDuration.FromTimeSpan(ts.ToTimeSpan());
    }

    public static TimeSpan ConvertToTimeSpan(WktDuration ts)
    {
        return ts.ToTimeSpan();
    }

    public static WktDuration ConvertToWktDuration(TimeSpan ts)
    {
        return WktDuration.FromTimeSpan(ts);
    }

    public static partial RocketType MapRocketType(Core.Domain.RocketType request);
    public static partial Core.Domain.RocketType MapRocketType(RocketType request);

    public static NullableRocketType MapNullableRocketType(Core.Domain.RocketType request)
    {
        return new() { Data = (RocketType)request, };
    }

    public static Core.Domain.RocketType MapNullableRocketType(NullableRocketType request)
    {
        return (Core.Domain.RocketType)request.Data;
    }

    public static NullableRocketType MapRocketType(Core.Domain.RocketType? request)
    {
        return request.HasValue ? new() { Data = (RocketType)request.Value, } : new() { Null = NullValue.NullValue, };
    }

    public static Core.Domain.RocketType? MapRocketType(NullableRocketType request)
    {
        return request.KindCase == NullableRocketType.KindOneofCase.Data
            ? (Core.Domain.RocketType)request.Data
            : default;
    }
}