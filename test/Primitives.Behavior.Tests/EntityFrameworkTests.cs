using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Primitives.Generated;

namespace Primitives.Behavior.Tests;

/// <summary>
///     EF Core through the EF Core template: <c>LaunchPadPrimitives.ConfigureDbContexts</c>, which the joint calls for every DbContext the
///     registry lists, and <c>ConfigureDomainPrimitives</c> for a context that configures itself.
/// </summary>
public class EntityFrameworkTests
{
    private static async Task<(ServiceProvider Provider, SqliteConnection Connection)> Configure()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var services = new ServiceCollection();
        services.AddDbContext<ShopContext>(options => options.UseSqlite(connection));
        // what LaunchPadPrimitivesEntityFrameworkJoint does
        LaunchPadPrimitives.ConfigureDbContexts(services);
        return (services.BuildServiceProvider(), connection);
    }

    [Test]
    public async Task Should_Store_Domain_Primitives_And_Read_Them_Back()
    {
        var (provider, connection) = await Configure();
        await using var _ = provider;
        await using var __ = connection;
        var order = new ShopOrder
        {
            Email = EmailAddress.Create("ada@example.com"),
            Family = LastName.Create("Lovelace"),
            Rating = MovieRating.Create(4.25m),
            Status = OrderStatus.Processing,
            Role = Role.Admin,
            Number = OrderNumber.Create(7),
            Stay = Stay.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)),
            Price = Cash.Create(10.5m, "CAD"),
            Day = CenturyDay.Create(new DateOnly(2026, 11, 23)),
        };

        await using (var scope = provider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
            await context.Database.EnsureCreatedAsync();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        // a key that is a strong ID of a Guid gets a new version 7 ID when it is added
        await order.Id.IsDefault.Should().BeFalse();
        await order.Id.Value.Version.Should().BeEqualTo(7);

        await using (var scope = provider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
            var email = EmailAddress.Create("ada@example.com");
            var read = await context.Orders.SingleAsync(z => z.Email == email);

            await read.Id.Should().BeEqualTo(order.Id);
            await read.Family.Should().BeEqualTo(order.Family);
            await read.Rating.Should().BeEqualTo(order.Rating);
            await read.Status.Should().BeEqualTo(OrderStatus.Processing);
            await read.Role.Should().BeEqualTo(Role.Admin);
            await read.Number.Should().BeEqualTo(order.Number);
            await read.Stay.Should().BeEqualTo(order.Stay);
            await read.Price.Should().BeEqualTo(order.Price);
            await read.Day.Should().BeEqualTo(order.Day);
            await ( await context.Orders.CountAsync(z => z.Status == OrderStatus.Pending) ).Should().BeEqualTo(0);
        }
    }

    [Test]
    public async Task Should_Map_The_Rules_And_The_Value_Objects()
    {
        var (provider, connection) = await Configure();
        await using var _ = provider;
        await using var __ = connection;
        await using var scope = provider.CreateAsyncScope();
        var entity = scope.ServiceProvider.GetRequiredService<ShopContext>().Model.FindEntityType(typeof(ShopOrder))!;

        // the converter carries the maximum length and the scale of the rules
        await entity.FindProperty(nameof(ShopOrder.Email))!.GetTypeMapping().Converter!.MappingHints!.Size.Should().BeEqualTo(320);
        await entity.FindProperty(nameof(ShopOrder.Rating))!.GetTypeMapping().Converter!.MappingHints!.Scale.Should().BeEqualTo(2);
        await entity.FindProperty(nameof(ShopOrder.Family))!.IsNullable.Should().BeTrue();
        // value objects are complex properties, not entities
        await entity.FindComplexProperty(nameof(ShopOrder.Stay)).Should().NotBeNull();
        await entity.FindComplexProperty(nameof(ShopOrder.Price)).Should().NotBeNull();
        await entity.FindPrimaryKey()!.Properties.Single().Name.Should().BeEqualTo(nameof(ShopOrder.Id));
    }

    [Test]
    public async Task Should_Configure_A_Context_That_Calls_ConfigureDomainPrimitives()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ConventionContext>().UseSqlite(connection).Options;

        await using (var context = new ConventionContext(options))
        {
            var entity = context.Model.FindEntityType(typeof(LedgerEntry))!;
            await entity.FindProperty(nameof(LedgerEntry.Email))!.GetMaxLength().Should().BeEqualTo(320);
            await entity.FindProperty(nameof(LedgerEntry.Rating))!.GetScale().Should().BeEqualTo(2);
            await entity.FindComplexProperty(nameof(LedgerEntry.Stay)).Should().NotBeNull();

            await context.Database.EnsureCreatedAsync();
            context.Entries.Add(
                new LedgerEntry
                {
                    Id = OrderNumber.Create(1),
                    Email = EmailAddress.Create("bea@example.com"),
                    Rating = MovieRating.Create(1.5m),
                    Stay = Stay.Create(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 2)),
                }
            );
            await context.SaveChangesAsync();
        }

        await using (var context = new ConventionContext(options))
        {
            var entry = await context.Entries.SingleAsync();
            await entry.Email.Should().BeEqualTo(EmailAddress.Create("bea@example.com"));
            await entry.Stay.Days.Should().BeEqualTo(2);
        }
    }
}

public sealed class ShopOrder
{
    public CustomerId Id { get; set; }
    public EmailAddress Email { get; set; }
    public LastName? Family { get; set; }
    public MovieRating Rating { get; set; }
    public OrderStatus Status { get; set; }
    public Role Role { get; set; } = Role.User;
    public OrderNumber Number { get; set; }
    public Stay Stay { get; set; }
    public Cash Price { get; set; }
    public CenturyDay Day { get; set; }
}

public sealed class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<ShopOrder> Orders => Set<ShopOrder>();
}

public sealed class LedgerEntry
{
    public OrderNumber Id { get; set; }
    public EmailAddress Email { get; set; }
    public MovieRating Rating { get; set; }
    public Stay Stay { get; set; }
}

/// <summary>
///     A context that isn't configured by the joint: it calls ConfigureDomainPrimitives itself.
/// </summary>
public sealed class ConventionContext(DbContextOptions<ConventionContext> options) : DbContext(options)
{
    public DbSet<LedgerEntry> Entries => Set<LedgerEntry>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) => configurationBuilder.ConfigureDomainPrimitives();
}
