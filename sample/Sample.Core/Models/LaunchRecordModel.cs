using System;
using AutoMapper;
using Bogus.Extensions;
using NodaTime;
using Sample.Core.Domain;

namespace Sample.Core.Models
{
    public record LaunchRecordModel
    {
        public Guid Id { get; init; }

        public string Partner { get; init; } = null!;
        public string Payload { get; init; } = null!;
        public long PayloadWeightKg { get; init; }
        public Instant? ActualLaunchDate { get; init; }
        public Instant ScheduledLaunchDate { get; init; }
        public string RocketSerialNumber { get; init; } = null!;
        public RocketType RocketType { get; init; }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<LaunchRecord, LaunchRecordModel>()
                   .ForMember(z => z.ActualLaunchDate, o => o.MapFrom(z => z.ActualLaunchDate.HasValue ? (Instant?)Instant.FromDateTimeOffset(z.ActualLaunchDate.Value) : default))
                   .ForMember(z => z.ScheduledLaunchDate, o => o.MapFrom(z => Instant.FromDateTimeOffset(z.ScheduledLaunchDate)))
                    ;
            }
        }
    }
}