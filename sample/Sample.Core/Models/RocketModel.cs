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
}