using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sample.Core.Domain;

/// <summary>
///     A rocket in inventory
/// </summary>
public class ReadyRocket
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = null!;
    public RocketType Type { get; set; }

    public IEnumerable<LaunchRecord> LaunchRecords { get; set; } = null!;

    private class EntityConfiguration : IEntityTypeConfiguration<ReadyRocket>
    {
        public void Configure(EntityTypeBuilder<ReadyRocket> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Rockets");
        }
    }
}
