using Microsoft.EntityFrameworkCore;

using Rocket.Surgery.LaunchPad.EntityFramework;

namespace EntityFramework.Tests;

public class StronglyTypedIdValueGeneratorTests
{
    [Test]
    public async Task Should_Generate_Each_Value_From_The_Factory()
    {
        var next = 0;
        var generator = StronglyTypedIdValueGenerator.Create(() => new RocketId(++next))(null!, null!);

        await generator.GeneratesTemporaryValues.Should().BeFalse();
        await generator.Next(null!).Should().BeEqualTo(new RocketId(1));
        await generator.Next(null!).Should().BeEqualTo(new RocketId(2));
    }

    [Test]
    public async Task Should_Give_A_New_Entity_Its_Id_When_It_Is_Added()
    {
        await using var context = new RocketContext(new DbContextOptionsBuilder<RocketContext>().UseSqlite("Data Source=:memory:").Options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var rocket = new Rocket { Name = "Saturn V", };
        context.Add(rocket);

        await rocket.Id.Should().BeEqualTo(RocketContext.GeneratedId);
        await context.SaveChangesAsync();
        await ( await context.Set<Rocket>().SingleAsync() ).Id.Should().BeEqualTo(RocketContext.GeneratedId);
    }

    private readonly record struct RocketId(int Value);

    private sealed class Rocket
    {
        public RocketId Id { get; set; }
        public required string Name { get; set; }
    }

    private sealed class RocketContext(DbContextOptions<RocketContext> options) : DbContext(options)
    {
        public static readonly RocketId GeneratedId = new(42);

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder
               .Entity<Rocket>()
               .Property(z => z.Id)
               .HasConversion(id => id.Value, value => new(value))
               .HasValueGenerator(StronglyTypedIdValueGenerator.Create(() => GeneratedId))
               .ValueGeneratedOnAdd();
    }
}
