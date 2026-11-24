using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NodaTime;

using Primitives.Behavior.Tests;

using Rocket.Surgery.LaunchPad.NodaTime;

namespace Primitives.Json.Tests;

/// <summary>
///     An application's source generated JsonSerializerContext, describing domain primitives declared in an assembly it references: the
///     System.Text.Json generator reads the [JsonConverter] attribute the Primitives generator wrote there and creates that converter. The
///     options resolve metadata from the context and, for the NodaTime values a NodaTime domain primitive holds, from LaunchPad.NodaTime's
///     resolver only, and reflection based serialization is turned off for this application (JsonSerializerIsReflectionEnabledByDefault), so
///     nothing here can fall back to reflection.
/// </summary>
public class JsonContextTests
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        TypeInfoResolver = PrimitivesJsonContext.Default,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    }.ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static async Task RoundTrips<T>(T value, string json)
    {
        await JsonSerializer.Serialize(value, Options).Should().BeEqualTo(json);
        await JsonSerializer.Deserialize<T>(json, Options).Should().BeEqualTo(value);
    }

    private static async Task<JsonException?> FailsToRead<T>(string json) =>
        await ( (Action)( () => JsonSerializer.Deserialize<T>(json, Options) ) ).Should().Throw<JsonException>();

    [Test]
    public async Task Should_Have_No_Reflection_To_Fall_Back_To()
    {
        await JsonSerializer.IsReflectionEnabledByDefault.Should().BeFalse();
        await Options.TypeInfoResolverChain.Count.Should().BeEqualTo(2);
        await ( Options.TypeInfoResolverChain[0] == PrimitivesJsonContext.Default ).Should().BeTrue();
        await ( Options.TypeInfoResolverChain[1] == NodaTimeSystemTextJsonSerializationExtensions.TypeInfoResolver ).Should().BeTrue();
        // a type the context doesn't describe has no metadata
        await ( (Action)( () => JsonSerializer.Serialize(new NotInTheContext(), Options) ) ).Should().Throw<NotSupportedException>();
    }

    [Test]
    public async Task Should_Describe_A_Domain_Primitive_By_The_Converter_The_Primitives_Generator_Wrote()
    {
        await ( Options.GetTypeInfo<CustomerId>().Converter is CustomerId.CustomerIdJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<LastName>().Converter is LastName.LastNameJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<OrderStatus>().Converter is OrderStatus.OrderStatusJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<DeliveryStatus>().Converter is DeliveryStatus.DeliveryStatusJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<Size>().Converter is Size.SizeJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<Address>().Converter is Address.AddressJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<Cash>().Converter is Cash.CashJsonConverter ).Should().BeTrue();
        await ( Options.GetTypeInfo<RecordedAt>().Converter is RecordedAt.RecordedAtJsonConverter ).Should().BeTrue();
        // not an object of its properties
        await Options.GetTypeInfo<Address>().Kind.Should().BeEqualTo(JsonTypeInfoKind.None);
    }

    [Test]
    public async Task Should_Read_And_Write_String_Primitives()
    {
        await RoundTrips(FirstName.Create("Ada"), "\"Ada\"");
        await RoundTrips(LastName.Create("Lovelace"), "\"Lovelace\"");
        await RoundTrips(EmailAddress.Create("ada@example.com"), "\"ada@example.com\"");
        await RoundTrips(Country.Create("CA"), "\"CA\"");
        await JsonSerializer.Deserialize<EmailAddress>("\"ADA@EXAMPLE.COM\"", Options).Value.Should().BeEqualTo("ada@example.com");
        await FailsToRead<EmailAddress>("\"nope\"");
        await JsonSerializer.Serialize(default(FirstName), Options).Should().BeEqualTo("null");
        await JsonSerializer.Deserialize<LastName>("null", Options).Should().BeNull();
    }

    [Test]
    public async Task Should_Read_And_Write_Numeric_Primitives()
    {
        await RoundTrips(Score.Create(42), "42");
        await RoundTrips(Points.Create(7), "7");
        await RoundTrips(CompletionRate.Create(12.5m), "12.5");
        await RoundTrips(Distance.Create(0.25), "0.25");
        await RoundTrips(Weight.Create(2.5m), "2.5");
        await RoundTrips<Score?>(null, "null");
        await RoundTrips<Score?>(Score.Create(1), "1");
        var error = await FailsToRead<Score>("101");
        await error!.Message.Should().Contain("[RANGE]");
    }

    [Test]
    public async Task Should_Read_And_Write_Date_Primitives()
    {
        await RoundTrips(CenturyDay.Create(new DateOnly(2026, 11, 23)), "\"2026-11-23\"");
        await RoundTrips(OfficeHour.Create(new TimeOnly(9, 30)), "\"09:30:00\"");
        await RoundTrips(Moment.Create(new DateTimeOffset(2026, 11, 23, 10, 30, 15, 123, TimeSpan.FromHours(-5))), "\"2026-11-23T10:30:15.123-05:00\"");
        await FailsToRead<CenturyDay>("\"1999-12-31\"");
    }

    [Test]
    public async Task Should_Read_And_Write_NodaTime_Primitives_Through_The_NodaTime_Converters()
    {
        await RoundTrips(RecordedAt.Create(Instant.FromUtc(2026, 11, 23, 10, 30)), "\"2026-11-23T10:30:00Z\"");
        await RoundTrips(ReportDay.Create(new LocalDate(2050, 6, 1)), "\"2050-06-01\"");
        await RoundTrips(AlarmTime.Create(new LocalTime(6, 45)), "\"06:45:00\"");
        await RoundTrips(Term.Create(Period.FromMonths(6)), "\"P6M\"");
        await RoundTrips(Elapsed.Create(Duration.FromMinutes(90)), "\"1:30:00\"");
        var offset = OffsetMoment.Create(new LocalDateTime(2026, 11, 23, 10, 30).WithOffset(Offset.FromHours(-5)));
        await JsonSerializer.Deserialize<OffsetMoment>(JsonSerializer.Serialize(offset, Options), Options).Should().BeEqualTo(offset);
        await FailsToRead<ReportDay>("\"1999-01-01\"");
    }

    [Test]
    public async Task Should_Read_And_Write_Strong_Ids()
    {
        await RoundTrips(CustomerId.Create(Guid.Parse("01936f3c-0000-7000-8000-000000000001")), "\"01936f3c-0000-7000-8000-000000000001\"");
        await RoundTrips(OrderNumber.Create(7), "7");
        await RoundTrips(TransactionId.Create(9_000_000_000), "9000000000");
        await RoundTrips(Sku.Create("ABC-1"), "\"ABC-1\"");
        await FailsToRead<CustomerId>("\"00000000-0000-0000-0000-000000000000\"");
    }

    [Test]
    public async Task Should_Read_And_Write_Smart_Enums()
    {
        await RoundTrips(OrderStatus.Processing, "\"processing\"");
        await RoundTrips(Role.ReadOnlyAuditor, "\"readOnlyAuditor\"");
        await JsonSerializer.Deserialize<OrderStatus>("3", Options).Should().BeEqualTo(OrderStatus.Completed);
        await FailsToRead<Role>("\"root\"");
        // a smart enum that is a readonly partial record struct, with data or without
        await RoundTrips(DeliveryStatus.Processing, "\"processing\"");
        await RoundTrips(Size.ExtraLarge, "\"extraLarge\"");
        await JsonSerializer.Deserialize<DeliveryStatus>("3", Options).Should().BeEqualTo(DeliveryStatus.Completed);
        await JsonSerializer.Deserialize<Size>("\"xl\"", Options).Label.Should().BeEqualTo("Extra large");
        await RoundTrips<Size?>(null, "null");
        await JsonSerializer.Serialize(default(DeliveryStatus), Options).Should().BeEqualTo("null");
        await FailsToRead<Size>("\"xxl\"");
    }

    [Test]
    public async Task Should_Read_And_Write_Value_Objects()
    {
        await RoundTrips(
            Address.Create("1 Main St", "Springfield", Country.Create("US"), 3),
            """{"street":"1 Main St","city":"Springfield","country":"US","floor":3}"""
        );
        // a NodaTime value of a value object goes through the NodaTime resolver, and a nullable one may be left out
        await RoundTrips(
            Contact.Create("Ada", EmailAddress.Create("ada@example.com"), null, null, new DateOnly(2020, 1, 2), new LocalDate(1815, 12, 10)),
            """{"name":"Ada","email":"ada@example.com","family":null,"country":null,"since":"2020-01-02","birthday":"1815-12-10"}"""
        );
        await RoundTrips(GeoPoint.Create(Latitude.Create(45.5), Longitude.Create(-73.5)), """{"latitude":45.5,"longitude":-73.5}""");
        await RoundTrips(GridCell.Create(0, 0), """{"row":0,"column":0}""");
        await JsonSerializer.Serialize(default(Address), Options).Should().BeEqualTo("null");
        var missing = await FailsToRead<Address>("""{"street":"1 Main St"}""");
        await missing!.Message.Should().Contain("[INVARIANT]");
        await FailsToRead<Household>("""{"country":"GB"}""");
    }

    [Test]
    public async Task Should_Read_And_Write_Money_And_Ranges()
    {
        await RoundTrips(Cash.Create(10.5m, "usd"), """{"amount":10.5,"currency":"USD"}""");
        await RoundTrips(Dollars.Create(3m), """{"amount":3,"currency":"USD"}""");
        await RoundTrips(Stay.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)), """{"start":"2026-01-01","end":"2026-01-03"}""");
        await RoundTrips(Semester.Create(new LocalDate(2026, 9, 1), new LocalDate(2026, 12, 20)), """{"start":"2026-09-01","end":"2026-12-20"}""");
        await RoundTrips(WorkShiftTime.Create(TimeOnly.MinValue, new TimeOnly(8, 0)), """{"start":"00:00:00","end":"08:00:00"}""");
        await RoundTrips(Shift.Create(new LocalTime(9, 0), new LocalTime(17, 0)), """{"start":"09:00:00","end":"17:00:00"}""");
        await FailsToRead<Cash>("""{"amount":1,"currency":"XYZ"}""");
        await FailsToRead<Stay>("""{"start":"2026-01-03","end":"2026-01-01"}""");
    }

    [Test]
    public async Task Should_Read_And_Write_Domain_Primitives_Inside_An_Object()
    {
        var order = new Order
        {
            Customer = CustomerId.Create(Guid.Parse("01936f3c-0000-7000-8000-000000000001")),
            Status = OrderStatus.Completed,
            Role = null,
            Delivery = DeliveryStatus.Processing,
            Size = Size.Medium,
            Total = Cash.Create(10m, "CAD"),
            Score = Score.Create(5),
            Stay = null,
            Skus = [Sku.Create("A-1"), Sku.Create("B-2")],
            Lines = new() { [OrderNumber.Create(1)] = ItemCount.Create(2) },
            RecordedAt = RecordedAt.Create(Instant.FromUtc(2026, 11, 23, 10, 30)),
        };
        const string json =
            """{"customer":"01936f3c-0000-7000-8000-000000000001","status":"completed","role":null,"delivery":"processing","size":"medium","total":{"amount":10,"currency":"CAD"},"score":5,"stay":null,"skus":["A-1","B-2"],"lines":{"1":2},"recordedAt":"2026-11-23T10:30:00Z"}""";

        await JsonSerializer.Serialize(order, Options).Should().BeEqualTo(json);
        var read = JsonSerializer.Deserialize<Order>(json, Options)!;
        await read.Customer.Should().BeEqualTo(order.Customer);
        await read.Total.Should().BeEqualTo(order.Total);
        await read.Delivery.Should().BeEqualTo(DeliveryStatus.Processing);
        await read.Size!.Value.Weight.Should().BeEqualTo(1m);
        await read.Skus.SequenceEqual(order.Skus).Should().BeTrue();
        await read.Lines[OrderNumber.Create(1)].Should().BeEqualTo(ItemCount.Create(2));
        await JsonSerializer.Serialize(read, Options).Should().BeEqualTo(json);
        // a dictionary key goes through the rules too
        await ( (Action)( () => JsonSerializer.Deserialize<Order>("""{"lines":{"0":1}}""", Options) ) ).Should().Throw<JsonException>();
    }
}

public sealed record Order
{
    public CustomerId Customer { get; init; }
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public Role? Role { get; init; }
    public DeliveryStatus Delivery { get; init; }
    public Size? Size { get; init; }
    public Cash Total { get; init; }
    public Score? Score { get; init; }
    public Stay? Stay { get; init; }
    public IReadOnlyList<Sku> Skus { get; init; } = [];
    public Dictionary<OrderNumber, ItemCount> Lines { get; init; } = [];
    public RecordedAt? RecordedAt { get; init; }
}

public sealed record NotInTheContext
{
    public int Value { get; init; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FirstName))]
[JsonSerializable(typeof(LastName))]
[JsonSerializable(typeof(EmailAddress))]
[JsonSerializable(typeof(Country))]
[JsonSerializable(typeof(Score))]
[JsonSerializable(typeof(Score?))]
[JsonSerializable(typeof(Points))]
[JsonSerializable(typeof(CompletionRate))]
[JsonSerializable(typeof(Distance))]
[JsonSerializable(typeof(Weight))]
[JsonSerializable(typeof(CenturyDay))]
[JsonSerializable(typeof(OfficeHour))]
[JsonSerializable(typeof(Moment))]
[JsonSerializable(typeof(RecordedAt))]
[JsonSerializable(typeof(ReportDay))]
[JsonSerializable(typeof(AlarmTime))]
[JsonSerializable(typeof(Term))]
[JsonSerializable(typeof(Elapsed))]
[JsonSerializable(typeof(OffsetMoment))]
[JsonSerializable(typeof(CustomerId))]
[JsonSerializable(typeof(OrderNumber))]
[JsonSerializable(typeof(TransactionId))]
[JsonSerializable(typeof(Sku))]
[JsonSerializable(typeof(OrderStatus))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(DeliveryStatus))]
[JsonSerializable(typeof(Size))]
[JsonSerializable(typeof(Size?))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(Contact))]
[JsonSerializable(typeof(GeoPoint))]
[JsonSerializable(typeof(GridCell))]
[JsonSerializable(typeof(Household))]
[JsonSerializable(typeof(Cash))]
[JsonSerializable(typeof(Dollars))]
[JsonSerializable(typeof(Stay))]
[JsonSerializable(typeof(Semester))]
[JsonSerializable(typeof(WorkShiftTime))]
[JsonSerializable(typeof(Shift))]
[JsonSerializable(typeof(Order))]
internal sealed partial class PrimitivesJsonContext : JsonSerializerContext;
