using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rocket.Surgery.LaunchPad.EntityFramework;
using Sample.Core.Models;

namespace Sample.Core.Domain;

public class LaunchRecord // : ILaunchRecord
{
    public LaunchRecordId Id { get; set; }
    [StringLength(1000)]
    public string? Partner { get; set; }
    [StringLength(1000)]
    public string? Payload { get; set; }
    public long PayloadWeightKg { get; set; }
    public DateTimeOffset? ActualLaunchDate { get; set; }
    public DateTimeOffset ScheduledLaunchDate { get; set; }

    public RocketId RocketId { get; set; }
    public ReadyRocket Rocket { get; set; } = null!;

    private class EntityConfiguration : IEntityTypeConfiguration<LaunchRecord>
    {
        public void Configure(EntityTypeBuilder<LaunchRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(z => z.Id)
                   .ValueGeneratedOnAdd()
                   .HasValueGenerator(StronglyTypedIdValueGenerator.Create(LaunchRecordId.New));
        }
    }
}

public interface ILaunchRecord
{
    LaunchRecordId Id { get; set; }
    string? Partner { get; set; }
    string? Payload { get; set; }
    long PayloadWeightKg { get; set; }
    DateTimeOffset? ActualLaunchDate { get; set; }
    DateTimeOffset ScheduledLaunchDate { get; set; }
}
