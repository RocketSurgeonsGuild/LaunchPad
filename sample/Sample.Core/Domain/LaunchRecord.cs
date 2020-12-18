using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;
using Rocket.Surgery.LaunchPad.EntityFramework;

namespace Sample.Core.Domain
{
    public class LaunchRecord
    {
        public Guid Id { get; set; }
        public string Partner { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public long PayloadWeightKg { get; set; }
        public DateTimeOffset? ActualLaunchDate { get; set; }
        public DateTimeOffset ScheduledLaunchDate { get; set; }

        public Guid RocketId { get; set; }
        public ReadyRocket Rocket { get; set; } = null!;

        class EntityConfiguration : IEntityTypeConfiguration<LaunchRecord>
        {
            public void Configure(EntityTypeBuilder<LaunchRecord> builder)
            {
                builder.HasKey(x => x.Id);
            }
        }
    }
}