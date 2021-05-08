using AutoMapper;
using NodaTime;
using Sample.Core.Domain;
using System;

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
                CreateMap<LaunchRecord, LaunchRecordModel>();
            }
        }
    }
}