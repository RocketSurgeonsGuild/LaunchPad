using System;
using AutoMapper;
using Sample.Core.Domain;

namespace Sample.Core.Models
{
    public record RocketModel
    {
        public Guid Id { get; init; }
        public string Sn { get; init; } = null!;
        public RocketType Type { get; init; }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<ReadyRocket, RocketModel>()
                   .ForMember(z => z.Sn, x => x.MapFrom(z => z.SerialNumber));
            }
        }
    }
}