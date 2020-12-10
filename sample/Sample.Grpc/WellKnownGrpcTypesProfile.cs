using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using NodaTime;
using NodaTime.Extensions;
using Duration = NodaTime.Duration;
using WktDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace Sample.Grpc
{
    public class WellKnownGrpcTypesProfile : Profile
    {
        public WellKnownGrpcTypesProfile()
        {
            CreateMap<Timestamp?, Instant>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default
                        : Instant.FromDateTimeOffset(ts.ToDateTimeOffset())
            );
            CreateMap<Timestamp?, Instant?>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default(Instant?)
                        : Instant.FromDateTimeOffset(ts.ToDateTimeOffset())
            );
            CreateMap<Instant, Timestamp>().ConvertUsing(
                static ts =>
                    Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset())
            );
            CreateMap<Instant?, Timestamp>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? Timestamp.FromDateTimeOffset(ts.Value.ToDateTimeOffset())
                        : new Timestamp() { Seconds = 0 }
            );

            CreateMap<NullableTimestamp?, Instant>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset())
                        : default
            );
            CreateMap<NullableTimestamp?, Instant?>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? Instant.FromDateTimeOffset(ts.Data.ToDateTimeOffset())
                        : default(Instant?)
            );
            CreateMap<Instant, NullableTimestamp>()
               .ConvertUsing(
                    static ts =>
                        new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(ts.ToDateTimeOffset()) }
                );
            CreateMap<Instant?, NullableTimestamp>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(ts.Value.ToDateTimeOffset()) }
                        : new NullableTimestamp() { Null = NullValue.NullValue }
            );

            CreateMap<Timestamp?, DateTimeOffset>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default
                        : ts.ToDateTimeOffset()
            );
            CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default(DateTimeOffset?)
                        : ts.ToDateTimeOffset()
            );
            CreateMap<DateTimeOffset, Timestamp>().ConvertUsing(
                static ts =>
                    Timestamp.FromDateTimeOffset(ts)
            );
            CreateMap<DateTimeOffset?, Timestamp>()
               .ConvertUsing(
                    static ts =>
                        ts.HasValue
                            ? Timestamp.FromDateTimeOffset(ts.Value)
                            : new Timestamp() { Seconds = 0 }
                );

            CreateMap<NullableTimestamp?, DateTimeOffset>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? ts.Data.ToDateTimeOffset()
                        : default
            );
            CreateMap<NullableTimestamp?, DateTimeOffset?>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? ts.Data.ToDateTimeOffset()
                        : default(DateTimeOffset?)
            );
            CreateMap<DateTimeOffset, NullableTimestamp>().ConvertUsing(
                static ts =>
                    new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(ts) }
            );
            CreateMap<DateTimeOffset?, NullableTimestamp>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(ts.Value) }
                        : new NullableTimestamp() { Null = NullValue.NullValue }
            );

            CreateMap<Timestamp?, DateTime>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default
                        : ts.ToDateTime()
            );
            CreateMap<Timestamp?, DateTime?>().ConvertUsing(
                static ts => ts == null
                    ? default(DateTime?)
                    : ts.ToDateTime()
            );
            CreateMap<DateTime, Timestamp>().ConvertUsing(
                static ts =>
                    Timestamp.FromDateTime(ts)
            );
            CreateMap<DateTime?, Timestamp>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? Timestamp.FromDateTime(ts.Value)
                        : new Timestamp() { Seconds = 0 }
            );

            CreateMap<NullableTimestamp?, DateTime>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? ts.Data.ToDateTime()
                        : default
            );
            CreateMap<NullableTimestamp?, DateTime?>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableTimestamp.KindOneofCase.Data
                        ? ts.Data.ToDateTime()
                        : default(DateTime?)
            );
            CreateMap<DateTime, NullableTimestamp>().ConvertUsing(
                static ts =>
                    new NullableTimestamp() { Data = Timestamp.FromDateTime(ts) }
            );
            CreateMap<DateTime?, NullableTimestamp>().ConvertUsing(
                static ts => ts.HasValue
                    ? new NullableTimestamp() { Data = Timestamp.FromDateTime(ts.Value) }
                    : new NullableTimestamp() { Null = NullValue.NullValue }
            );

            CreateMap<WktDuration?, Duration>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default
                        : Duration.FromTimeSpan(ts.ToTimeSpan())
            );
            CreateMap<WktDuration?, Duration?>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default(Duration?)
                        : Duration.FromTimeSpan(ts.ToTimeSpan())
            );
            CreateMap<Duration, WktDuration>().ConvertUsing(
                static ts =>
                    WktDuration.FromTimeSpan(ts.ToTimeSpan())
            );
            CreateMap<Duration?, WktDuration>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? WktDuration.FromTimeSpan(ts.Value.ToTimeSpan())
                        : new WktDuration() { Seconds = 0 }
            );

            CreateMap<WktDuration?, TimeSpan>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default
                        : ts.ToTimeSpan()
            );
            CreateMap<WktDuration?, TimeSpan?>().ConvertUsing(
                static ts =>
                    ts == null
                        ? default(TimeSpan?)
                        : ts.ToTimeSpan()
            );
            CreateMap<TimeSpan, WktDuration>().ConvertUsing(
                static ts =>
                    WktDuration.FromTimeSpan(ts)
            );
            CreateMap<TimeSpan?, WktDuration>().ConvertUsing(
                static ts =>
                    ts.HasValue
                        ? WktDuration.FromTimeSpan(ts.Value)
                        : new WktDuration() { Seconds = 0 }
            );
        }
    }
}