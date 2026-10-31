using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using NodaTime.Testing;

using Rocket.Surgery.LaunchPad.EntityFramework;

namespace NodaTime.EntityFramework.Tests;

public class NodaTimeValueGeneratorTests
{
    [Test]
    public async Task Generates_A_Date_Time_Offset_From_The_Clock()
    {
        var clock = new FakeClock(Instant.FromUtc(2020, 1, 1, 12, 12, 12));
        await using var context = await TestContext.Create(clock);

        var entity = new Stamped();
        context.Add(entity);

        entity.CreatedAt.ShouldBe(new DateTimeOffset(2020, 1, 1, 12, 12, 12, TimeSpan.Zero));
    }

    [Test]
    public async Task Generates_A_Utc_Date_Time_From_The_Clock()
    {
        var clock = new FakeClock(Instant.FromUtc(2020, 1, 1, 12, 12, 12));
        await using var context = await TestContext.Create(clock);

        var entity = new Stamped();
        context.Add(entity);

        entity.UpdatedAt.ShouldBe(new DateTime(2020, 1, 1, 12, 12, 12, DateTimeKind.Utc));
        entity.UpdatedAt.Kind.ShouldBe(DateTimeKind.Utc);
    }

    [Test]
    public void Generates_Permanent_Values()
    {
        var clock = new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0));

        new NodaTimeDateTimeOffsetValueGenerator(clock).GeneratesTemporaryValues.ShouldBeFalse();
        new NodaTimeDateTimeValueGenerator(clock).GeneratesTemporaryValues.ShouldBeFalse();
    }

    public class Stamped
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    private sealed class TestContext(DbContextOptions<TestContext> options, IClock clock) : DbContext(options)
    {
        public static async Task<TestContext> Create(IClock clock)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<TestContext>().UseSqlite(connection).Options;
            var context = new TestContext(options, clock);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Stamped>();
            entity.Property(x => x.CreatedAt).HasValueGenerator((_, _) => new NodaTimeDateTimeOffsetValueGenerator(clock)).ValueGeneratedOnAdd();
            entity.Property(x => x.UpdatedAt).HasValueGenerator((_, _) => new NodaTimeDateTimeValueGenerator(clock)).ValueGeneratedOnAdd();
        }
    }
}
