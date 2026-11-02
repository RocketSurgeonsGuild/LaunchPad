using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Rocket.Surgery.LaunchPad.EntityFramework;

namespace EntityFramework.Tests;

public class LpContextTests
{
    [Test]
    public void Should_Apply_The_Entity_Configurations_Of_The_Context_Assembly()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(Launch)).ShouldNotBeNull();

        entity.GetTableName().ShouldBe("launches");
        entity.FindProperty(nameof(Launch.Name)).ShouldNotBeNull().GetMaxLength().ShouldBe(64);
    }

    [Test]
    [Arguments(nameof(Launch.ScheduledAt))]
    [Arguments(nameof(Launch.LaunchedAt))]
    public void Should_Store_A_DateTimeOffset_As_Binary_On_Sqlite(string property)
    {
        using var context = CreateContext();

        context
           .Model.FindEntityType(typeof(Launch))!
           .FindProperty(property)
           .ShouldNotBeNull()
           .GetValueConverter()
           .ShouldBeOfType<DateTimeOffsetToBinaryConverter>();
    }

    [Test]
    [Arguments(nameof(Launch.CreatedAt))]
    [Arguments(nameof(Launch.UpdatedAt))]
    public void Should_Store_A_DateTime_As_Binary_On_Sqlite(string property)
    {
        using var context = CreateContext();

        context
           .Model.FindEntityType(typeof(Launch))!
           .FindProperty(property)
           .ShouldNotBeNull()
           .GetValueConverter()
           .ShouldBeOfType<DateTimeToBinaryConverter>();
    }

    [Test]
    public void Should_Leave_Other_Properties_Unconverted()
    {
        using var context = CreateContext();

        context.Model.FindEntityType(typeof(Launch))!.FindProperty(nameof(Launch.Name))!.GetValueConverter().ShouldBeNull();
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
        launch.ScheduledAt.ShouldBe(scheduledAt);
        launch.LaunchedAt.ShouldBeNull();
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
