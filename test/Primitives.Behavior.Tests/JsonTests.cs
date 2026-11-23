using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NodaTime;

using Rocket.Surgery.LaunchPad.NodaTime;

namespace Primitives.Behavior.Tests;

/// <summary>
///     The System.Text.Json converters the generator writes. <see cref="Options" /> has no reflection based resolver: each domain primitive's
///     metadata is made from an instance of its converter alone (<see cref="ConverterResolver" />), and a NodaTime value goes through the
///     options' own NodaTime converters, which ConfigureNodaTimeForLaunchPad adds, so the values these tests read and write go through no
///     reflection based metadata, as in a trimmed or native AOT application (ADR-0007). Collections and plain objects need the reflection based
///     resolver of <see cref="ReflectionOptions" />.
/// </summary>
/// <remarks>
///     Not a source generated JsonSerializerContext: the System.Text.Json generator doesn't see the [JsonConverter] attribute the Primitives
///     generator writes in the same project (generators don't see each other's output), so it would describe a domain primitive as an object.
///     Primitives.Json.Tests proves the source generated context of an application that references the domain primitives' assembly.
/// </remarks>
public class JsonTests
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { TypeInfoResolver = new ConverterResolver() }
       .ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static readonly JsonSerializerOptions ReflectionOptions = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }
       .ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static async Task RoundTrips<T>(T value, string json)
    {
        await JsonSerializer.Serialize(value, Options).Should().BeEqualTo(json);
        await JsonSerializer.Deserialize<T>(json, Options).Should().BeEqualTo(value);
    }

    private static Task<JsonException?> FailsToRead<T>(string json) =>
        PrimitiveAssert.Throws<JsonException>(() => JsonSerializer.Deserialize<T>(json, Options));

    [Test]
    public async Task Should_Write_A_Domain_Primitive_As_Its_Value()
    {
        await RoundTrips(FirstName.Create("Ada"), "\"Ada\"");
        await RoundTrips(LastName.Create("Lovelace"), "\"Lovelace\"");
        await RoundTrips(Score.Create(42), "42");
        await RoundTrips(Points.Create(7), "7");
        await RoundTrips(CompletionRate.Create(12.5m), "12.5");
        await RoundTrips(Distance.Create(0.25), "0.25");
        await RoundTrips(CenturyDay.Create(new DateOnly(2026, 11, 23)), "\"2026-11-23\"");
        await RoundTrips(OfficeHour.Create(new TimeOnly(9, 30)), "\"09:30:00\"");
        var guid = Guid.Parse("01936f3c-0000-7000-8000-000000000001");
        await RoundTrips(CustomerId.Create(guid), "\"01936f3c-0000-7000-8000-000000000001\"");
        await RoundTrips(OrderNumber.Create(7), "7");
    }

    [Test]
    public async Task Should_Read_A_Value_Through_Its_Rules()
    {
        await JsonSerializer.Deserialize<EmailAddress>("\"ADA@EXAMPLE.COM\"", Options).Value.Should().BeEqualTo("ada@example.com");
        var error = await FailsToRead<Score>("101");
        await error!.Message.Should().Contain("[RANGE]");
        await FailsToRead<EmailAddress>("\"nope\"");
        await FailsToRead<CustomerId>("\"00000000-0000-0000-0000-000000000000\"");
        // escapes are decoded before the value is checked
        await JsonSerializer.Deserialize<FirstName>("\"Jos\\u00e9\"", Options).Value.Should().BeEqualTo("José");
    }

    [Test]
    public async Task Should_Write_Null_For_A_Missing_Value()
    {
        // a struct: the default value is null, and null reads as the default value
        await JsonSerializer.Serialize(default(FirstName), Options).Should().BeEqualTo("null");
        await JsonSerializer.Serialize(default(Score), Options).Should().BeEqualTo("null");
        await JsonSerializer.Deserialize<FirstName>("null", Options).IsDefault.Should().BeTrue();
        await JsonSerializer.Deserialize<Score?>("null", Options).Should().BeNull();
        // a class: null is a null reference
        await JsonSerializer.Serialize<LastName?>(null, Options).Should().BeEqualTo("null");
        await JsonSerializer.Deserialize<LastName>("null", Options).Should().BeNull();
    }

    [Test]
    public async Task Should_Write_A_Smart_Enum_As_Its_Name()
    {
        await RoundTrips(OrderStatus.Processing, "\"processing\"");
        await RoundTrips(Role.ReadOnlyAuditor, "\"readOnlyAuditor\"");
        // read by its name in any case, or by its value
        await JsonSerializer.Deserialize<OrderStatus>("\"COMPLETED\"", Options).Should().BeEqualTo(OrderStatus.Completed);
        await JsonSerializer.Deserialize<OrderStatus>("3", Options).Should().BeEqualTo(OrderStatus.Completed);
        await JsonSerializer.Deserialize<Role>("\"admin\"", Options).Should().BeEqualTo(Role.Admin);
        await FailsToRead<OrderStatus>("42");
        await FailsToRead<Role>("\"root\"");
        await JsonSerializer.Serialize(default(OrderStatus), Options).Should().BeEqualTo("null");
    }

    [Test]
    public async Task Should_Write_A_Value_Object_As_An_Object()
    {
        await RoundTrips(
            Address.Create("1 Main St", "Springfield", Country.Create("US"), 3),
            """{"street":"1 Main St","city":"Springfield","country":"US","floor":3}"""
        );
        // null values are written, and read back as null
        await RoundTrips(Address.Create("1 Main St", null, Country.Create("US"), null), """{"street":"1 Main St","city":null,"country":"US","floor":null}""");
        await RoundTrips(
            Contact.Create("Ada", EmailAddress.Create("ada@example.com"), null, null, new DateOnly(2020, 1, 2), new LocalDate(1815, 12, 10)),
            """{"name":"Ada","email":"ada@example.com","family":null,"country":null,"since":"2020-01-02","birthday":"1815-12-10"}"""
        );
        await RoundTrips(
            Contact.Create("Ada", null, LastName.Create("Lovelace"), Country.Create("GB"), new DateOnly(2020, 1, 2), null),
            """{"name":"Ada","email":null,"family":"Lovelace","country":"GB","since":"2020-01-02","birthday":null}"""
        );
        await RoundTrips(GeoPoint.Create(Latitude.Create(45.5), Longitude.Create(-73.5)), """{"latitude":45.5,"longitude":-73.5}""");
    }

    [Test]
    public async Task Should_Read_A_Value_Object_In_Any_Case_And_Skip_Unknown_Values()
    {
        var address = JsonSerializer.Deserialize<Address>("""{"STREET":"1 Main St","Country":"us","extra":{"a":[1,2]}}""", Options);

        await address.Should().BeEqualTo(Address.Create("1 Main St", null, Country.Create("US"), null));
    }

    [Test]
    public async Task Should_Fail_To_Read_A_Value_Object_Its_Rules_Reject()
    {
        var invariant = await FailsToRead<Address>("""{"street":" ","country":"US"}""");
        await invariant!.Message.Should().Contain("[INVARIANT]");
        // a value one of its domain primitives rejects
        var nested = await FailsToRead<Address>("""{"street":"1 Main St","country":"ZZ"}""");
        await nested!.Message.Should().Contain("[FORMAT]");
        await FailsToRead<Address>("[]");
        await FailsToRead<Stay>("""{"start":"2026-01-03","end":"2026-01-01"}""");
    }

    [Test]
    public async Task Should_Fail_To_Read_A_Value_Object_Without_A_Domain_Primitive_It_Requires()
    {
        // the country is a non-nullable domain primitive: left out, it has no value
        var missing = await FailsToRead<Address>("""{"street":"1 Main St"}""");
        await missing!.Message.Should().BeEqualTo("[INVARIANT] Address.Country must have a value.");
        // null, for a domain primitive that isn't nullable, is a value its own rules reject
        var nulled = await FailsToRead<Address>("""{"street":"1 Main St","country":null}""");
        await nulled!.Message.Should().BeEqualTo("[NULL_INPUT] Country must not be null.");
        // a class value object: a class domain primitive left out, or null
        var family = await FailsToRead<Household>("""{"country":"GB"}""");
        await family!.Message.Should().BeEqualTo("[INVARIANT] Household.Family must have a value.");
        await FailsToRead<Household>("""{"family":null,"country":"GB"}""");
        await RoundTrips(Household.Create(LastName.Create("Lovelace"), CountryClass.Create("GB")), """{"family":"Lovelace","country":"GB"}""");
    }

    [Test]
    public async Task Should_Read_A_Nullable_Domain_Primitive_Left_Out_As_Null()
    {
        var contact = JsonSerializer.Deserialize<Contact>("""{"name":"Ada","since":"2020-01-02"}""", Options)!;

        await contact.Email.Should().BeNull();
        await contact.Family.Should().BeNull();
        await contact.Country.Should().BeNull();
        await contact.Birthday.Should().BeNull();
        await RoundTrips(contact, """{"name":"Ada","email":null,"family":null,"country":null,"since":"2020-01-02","birthday":null}""");
    }

    [Test]
    public async Task Should_Write_A_Default_Struct_Value_Object_As_Its_Values()
    {
        // midnight to midnight is a range like any other, and so is the default value of a value object of plain values
        await RoundTrips(WorkShiftTime.Create(TimeOnly.MinValue, TimeOnly.MinValue), """{"start":"00:00:00","end":"00:00:00"}""");
        await JsonSerializer.Serialize(default(WorkShiftTime), Options).Should().BeEqualTo("""{"start":"00:00:00","end":"00:00:00"}""");
        await RoundTrips(GridCell.Create(0, 0), """{"row":0,"column":0}""");
        await RoundTrips(default(GridCell), """{"row":0,"column":0}""");
        await RoundTrips(GridCell.Create(2, 3), """{"row":2,"column":3}""");
    }

    [Test]
    public async Task Should_Write_Null_For_A_Default_Struct_Value_Object_That_Holds_A_Domain_Primitive()
    {
        // default(Address) holds a Country without a value, and default(GeoPoint) a Latitude and a Longitude without one: neither can be
        // written as its values, so each is null, which reads back as the default value
        await JsonSerializer.Serialize(default(Address), Options).Should().BeEqualTo("null");
        await JsonSerializer.Serialize(default(GeoPoint), Options).Should().BeEqualTo("null");
        await JsonSerializer.Deserialize<Address>("null", Options).IsDefault.Should().BeTrue();
        await JsonSerializer.Deserialize<GeoPoint>("null", Options).IsDefault.Should().BeTrue();
    }

    [Test]
    public async Task Should_Write_The_Kinds_Of_Value_Objects()
    {
        await RoundTrips(Cash.Create(10.5m, "usd"), """{"amount":10.5,"currency":"USD"}""");
        await RoundTrips(Dollars.Create(3m), """{"amount":3,"currency":"USD"}""");
        await RoundTrips(Stay.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)), """{"start":"2026-01-01","end":"2026-01-03"}""");
        await RoundTrips(Semester.Create(new LocalDate(2026, 9, 1), new LocalDate(2026, 12, 20)), """{"start":"2026-09-01","end":"2026-12-20"}""");
        await RoundTrips(WorkShiftTime.Create(TimeOnly.MinValue, new TimeOnly(8, 0)), """{"start":"00:00:00","end":"08:00:00"}""");
        await RoundTrips(Shift.Create(new LocalTime(9, 0), new LocalTime(17, 0)), """{"start":"09:00:00","end":"17:00:00"}""");
        // a default struct is written as its values: null comes only from a nullable value object
        await JsonSerializer.Serialize(default(Cash), Options).Should().BeEqualTo("""{"amount":0,"currency":null}""");
    }

    [Test]
    public async Task Should_Name_The_Values_Of_A_Value_Object_By_The_Naming_Policy()
    {
        var options = new JsonSerializerOptions(Options) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var json = JsonSerializer.Serialize(Cash.Create(1m, "EUR"), options);

        await json.Should().BeEqualTo("""{"amount":1,"currency":"EUR"}""");
        await JsonSerializer.Serialize(Address.Create("1 Main St", null, Country.Create("US"), null), options)
                            .Should()
                            .BeEqualTo("""{"street":"1 Main St","city":null,"country":"US","floor":null}""");
    }

    [Test]
    public async Task Should_Write_A_NodaTime_Primitive_Through_The_NodaTime_Converters()
    {
        await RoundTrips(RecordedAt.Create(Instant.FromUtc(2026, 11, 23, 10, 30)), "\"2026-11-23T10:30:00Z\"");
        await RoundTrips(ReportDay.Create(new LocalDate(2050, 6, 1)), "\"2050-06-01\"");
        await RoundTrips(AlarmTime.Create(new LocalTime(6, 45)), "\"06:45:00\"");
        await RoundTrips(Term.Create(Period.FromMonths(6)), "\"P6M\"");
        await RoundTrips(Elapsed.Create(Duration.FromMinutes(90)), "\"1:30:00\"");
        await FailsToRead<ReportDay>("\"1999-01-01\"");
    }

    [Test]
    public async Task Should_Use_A_Domain_Primitive_As_A_Dictionary_Key()
    {
        var guid = Guid.Parse("01936f3c-0000-7000-8000-000000000001");
        var byId = new Dictionary<CustomerId, int> { [CustomerId.Create(guid)] = 1 };
        var byStatus = new Dictionary<OrderStatus, int> { [OrderStatus.Pending] = 2 };
        var byDay = new Dictionary<CenturyDay, int> { [CenturyDay.Create(new DateOnly(2026, 1, 2))] = 3 };
        var byTime = new Dictionary<OfficeHour, int> { [OfficeHour.Create(new TimeOnly(9, 30, 15))] = 4 };

        await JsonSerializer.Serialize(byId, ReflectionOptions).Should().BeEqualTo("""{"01936f3c-0000-7000-8000-000000000001":1}""");
        await JsonSerializer.Deserialize<Dictionary<CustomerId, int>>("""{"01936f3c-0000-7000-8000-000000000001":1}""", ReflectionOptions)!
                            [CustomerId.Create(guid)]
                            .Should()
                            .BeEqualTo(1);
        await JsonSerializer.Serialize(byStatus, ReflectionOptions).Should().BeEqualTo("""{"pending":2}""");
        await JsonSerializer.Deserialize<Dictionary<OrderStatus, int>>("""{"PENDING":2}""", ReflectionOptions)![OrderStatus.Pending].Should().BeEqualTo(2);
        await JsonSerializer.Serialize(byDay, ReflectionOptions).Should().BeEqualTo("""{"2026-01-02":3}""");
        // a key keeps every part of its value
        var time = JsonSerializer.Deserialize<Dictionary<OfficeHour, int>>(JsonSerializer.Serialize(byTime, ReflectionOptions), ReflectionOptions)!;
        await time.Keys.Single().Value.Should().BeEqualTo(new TimeOnly(9, 30, 15));
        await PrimitiveAssert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dictionary<Score, int>>("""{"500":1}""", ReflectionOptions));
    }

    [Test]
    public async Task Should_Write_A_Domain_Primitive_Inside_An_Object()
    {
        var order = new OrderDocument
        {
            Id = CustomerId.Create(Guid.Parse("01936f3c-0000-7000-8000-000000000001")),
            Email = EmailAddress.Create("ada@example.com"),
            Status = OrderStatus.Completed,
            Price = Cash.Create(10m, "CAD"),
            Score = null,
            Family = null,
        };

        var json = JsonSerializer.Serialize(order, ReflectionOptions);

        await json.Should()
                  .BeEqualTo(
                       """{"Id":"01936f3c-0000-7000-8000-000000000001","Email":"ada@example.com","Status":"completed","Price":{"amount":10,"currency":"CAD"},"Score":null,"Family":null}"""
                   );
        await JsonSerializer.Deserialize<OrderDocument>(json, ReflectionOptions).Should().BeEqualTo(order);
    }
}

public sealed record OrderDocument
{
    public CustomerId Id { get; init; }
    public EmailAddress Email { get; init; }
    public OrderStatus Status { get; init; }
    public Cash Price { get; init; }
    public Score? Score { get; init; }
    public LastName? Family { get; init; }
}

/// <summary>
///     Describes each domain primitive by the converter the generator wrote for it, and nothing else: options with only this resolver (and the
///     NodaTime one) have no reflection based metadata to fall back to.
/// </summary>
internal sealed class ConverterResolver : IJsonTypeInfoResolver
{
    private static readonly Dictionary<Type, Func<JsonSerializerOptions, JsonTypeInfo>> Factories = [];

    static ConverterResolver()
    {
        Add(new FirstName.FirstNameJsonConverter());
        Add(new LastName.LastNameJsonConverter());
        Add(new EmailAddress.EmailAddressJsonConverter());
        Add(new Score.ScoreJsonConverter());
        Add(new Points.PointsJsonConverter());
        Add(new CompletionRate.CompletionRateJsonConverter());
        Add(new Distance.DistanceJsonConverter());
        Add(new CenturyDay.CenturyDayJsonConverter());
        Add(new OfficeHour.OfficeHourJsonConverter());
        Add(new CustomerId.CustomerIdJsonConverter());
        Add(new OrderNumber.OrderNumberJsonConverter());
        Add(new OrderStatus.OrderStatusJsonConverter());
        Add(new Role.RoleJsonConverter());
        Add(new Address.AddressJsonConverter());
        Add(new Contact.ContactJsonConverter());
        Add(new GeoPoint.GeoPointJsonConverter());
        Add(new Household.HouseholdJsonConverter());
        Add(new GridCell.GridCellJsonConverter());
        Add(new CountryClass.CountryClassJsonConverter());
        Add(new Cash.CashJsonConverter());
        Add(new Dollars.DollarsJsonConverter());
        Add(new Stay.StayJsonConverter());
        Add(new Semester.SemesterJsonConverter());
        Add(new WorkShiftTime.WorkShiftTimeJsonConverter());
        Add(new Shift.ShiftJsonConverter());
        Add(new RecordedAt.RecordedAtJsonConverter());
        Add(new ReportDay.ReportDayJsonConverter());
        Add(new AlarmTime.AlarmTimeJsonConverter());
        Add(new Term.TermJsonConverter());
        Add(new Elapsed.ElapsedJsonConverter());
        Factories[typeof(Score?)] = static options => JsonMetadataServices.CreateValueInfo<Score?>(
            options,
            JsonMetadataServices.GetNullableConverter(options.GetTypeInfo<Score>())
        );
    }

    // The converter the type's [JsonConverter] attribute names, created as the source generated context of an application creates it: by its
    // name, not by reading the attribute.
    private static void Add<T>(JsonConverter<T> converter) => Factories[typeof(T)] = options => JsonMetadataServices.CreateValueInfo<T>(options, converter);

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) => Factories.TryGetValue(type, out var factory) ? factory(options) : null;
}
