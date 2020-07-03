using System;
using AutoMapper;
using Sample.Core.Domain;

namespace Sample.Core.Models
{
    public class RocketModel
    {
        public Guid Id { get; set; }
        public string Sn { get; set; } = null!;
        public RocketType Type { get; set; }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<ReadyRocket, RocketModel>()
                   .ForMember(z => z.Sn, x => x.MapFrom(z => z.SerialNumber));
            }
        }
    }

    public class LaunchRecordModel
    {
        public Guid Id { get; set; }

        public string Partner { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public long PayloadWeightKg { get; set; }
        public DateTimeOffset? ActualLaunchDate { get; set; }
        public DateTimeOffset ScheduledLaunchDate { get; set; }
        public string RocketSerialNumber { get; set; } = null!;
        public RocketType RocketType { get; set; }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<LaunchRecord, LaunchRecordModel>();
            }
        }
    }
}