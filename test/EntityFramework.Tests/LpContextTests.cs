using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Rocket.Surgery.LaunchPad.EntityFramework;

namespace EntityFramework.Tests;

public class LpContextTests
{
    [Test]
    public async Task Should_Apply_The_Entity_Configurations_Of_The_Context_Assembly()
    {
        using var context = CreateContext();

        var entity = await context.Model.FindEntityType(typeof(Launch)).Should().NotBeNull();

        await entity.GetTableName().Should().BeEqualTo("launches");
        await ( await entity.FindProperty(nameof(Launch.Name)).Should().NotBeNull() )!.GetMaxLength().Should().BeEqualTo(64);
    }

    [Test]
    [Arguments(nameof(Launch.ScheduledAt))]
    [Arguments(nameof(Launch.LaunchedAt))]
    public async Task Should_Store_A_DateTimeOffset_As_Binary_On_Sqlite(string property)
    {
        using var context = CreateContext();

        await ( await context
           .Model.FindEntityType(typeof(Launch))!
           .FindProperty(property)
           .Should().NotBeNull() )!
           .GetValueConverter()
           .Should().BeOfType(typeof(DateTimeOffsetToBinaryConverter));
    }

    [Test]
    [Arguments(nameof(Launch.CreatedAt))]
    [Arguments(nameof(Launch.UpdatedAt))]
    public async Task Should_Store_A_DateTime_As_Binary_On_Sqlite(string property)
    {
        using var context = CreateContext();

        await ( await context
           .Model.FindEntityType(typeof(Launch))!
           .FindProperty(property)
           .Should().NotBeNull() )!
           .GetValueConverter()
           .Should().BeOfType(typeof(DateTimeToBinaryConverter));
    }

    [Test]
    public async Task Should_Leave_Other_Properties_Unconverted()
    {
        using var context = CreateContext();

        await context.Model.FindEntityType(typeof(Launch))!.FindProperty(nameof(Launch.Name))!.GetValueConverter().Should().BeNull();
    }

    [Test]
    public async Task Should_Round_Trip_Dates_Through_Sqlite()
    {
        await using var context = CreateContext();
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();
        var scheduledAt = new DateTimeOffset(2026, 11, 1, 10, 30, 0, TimeSpan.FromHours(-5));

        context.Add(new Launch { Name = "Artemis", ScheduledAt = scheduledAt, CreatedAt = new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var launch = await context.Set<Launch>().SingleAsync(z => z.ScheduledAt == scheduledAt);
        await launch.ScheduledAt.Should().BeEqualTo(scheduledAt);
        await launch.LaunchedAt.Should().BeNull();
    }

    private static LaunchContext CreateContext() => new(new DbContextOptionsBuilder<LaunchContext>().UseSqlite("Data Source=:memory:").Options);

    private sealed class LaunchContext(DbContextOptions<LaunchContext> options) : LpContext<LaunchContext>(options);

    private sealed class Launch
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public DateTimeOffset? LaunchedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    [UsedImplicitly]
    private sealed class LaunchConfiguration : IEntityTypeConfiguration<Launch>
    {
        public void Configure(EntityTypeBuilder<Launch> builder)
        {
            builder.ToTable("launches");
            builder.Property(z => z.Name).HasMaxLength(64);
        }
    }
}
