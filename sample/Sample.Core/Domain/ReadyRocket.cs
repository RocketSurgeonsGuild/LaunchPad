using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rocket.Surgery.LaunchPad.EntityFramework;
using Sample.Core.Models;

namespace Sample.Core.Domain;

/// <summary>
///     A rocket in inventory
/// </summary>
public class ReadyRocket // : IReadyRocket
{
    public RocketId Id { get; set; }
    [StringLength(30)] public string SerialNumber { get; set; } = null!;
    public RocketType Type { get; set; }

    public IEnumerable<LaunchRecord> LaunchRecords { get; set; } = null!;

    private class EntityConfiguration : IEntityTypeConfiguration<ReadyRocket>
    {
        public void Configure(EntityTypeBuilder<ReadyRocket> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(z => z.Id)
                   .ValueGeneratedOnAdd()
#if NET8_0_OR_GREATER
                   .HasValueGenerator((property, @base) => StronglyTypedIdValueGenerator.Create(RocketId.New)(property, @base.ContainingEntityType));
#else
                   .HasValueGenerator(StronglyTypedIdValueGenerator.Create(RocketId.New));
#endif
            builder.ToTable("Rockets");
        }
    }
}

public interface IReadyRocket
{
    RocketId Id { get; set; }
    string SerialNumber { get; set; }
    RocketType Type { get; set; }
}
