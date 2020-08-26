using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using NodaTime;

namespace Sample.Grpc
{
    public class WellKnownGrpcTypesProfile : Profile
    {
        public WellKnownGrpcTypesProfile()
        {
            CreateMap<Timestamp, DateTimeOffset>()
               .ConvertUsing(z => z.ToDateTimeOffset());

            CreateMap<DateTimeOffset, Timestamp>()
               .ConvertUsing(z => Timestamp.FromDateTimeOffset(z));

            CreateMap<Timestamp, Instant>()
               .ConvertUsing(z => Instant.FromDateTimeOffset(z.ToDateTimeOffset()));

            CreateMap<Instant, Timestamp>()
               .ConvertUsing(z => Timestamp.FromDateTimeOffset(z.ToDateTimeOffset()));

            CreateMap<NullableTimestamp, DateTimeOffset?>()
               .ConvertUsing(z => z.KindCase == NullableTimestamp.KindOneofCase.Data ? (DateTimeOffset?)z.Data.ToDateTimeOffset() : null);

            CreateMap<DateTimeOffset?, NullableTimestamp>()
               .ConvertUsing(
                    z => z.HasValue
                        ? new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(z.Value) }
                        : new NullableTimestamp() { Null = NullValue.NullValue }
                );

            CreateMap<NullableTimestamp, Instant?>()
               .ConvertUsing(z => z.KindCase == NullableTimestamp.KindOneofCase.Data ? (Instant?)Instant.FromDateTimeOffset(z.Data.ToDateTimeOffset()) : null);

            CreateMap<Instant?, NullableTimestamp>()
               .ConvertUsing(
                    z => z.HasValue
                        ? new NullableTimestamp() { Data = Timestamp.FromDateTimeOffset(z.Value.ToDateTimeOffset()) }
                        : new NullableTimestamp() { Null = NullValue.NullValue }
                );
        }
    }
}