using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using CsCheck;

using NodaTime;

using Rocket.Surgery.LaunchPad.NodaTime;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

/// <summary>
///     Properties every domain primitive keeps, checked with CsCheck over generated values: a failing case is shrunk to the smallest value
///     that breaks it, and Sample throws with it.
/// </summary>
public class PropertyTests
{
    private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }
       .ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static readonly Gen<string> Text = Gen.String[Gen.Char["abcXYZ019 -_.\té́"], 0, 40];

    /// <summary>
    ///     Create accepts exactly what TryCreate accepts, and holds the same value.
    /// </summary>
    private static void CreateAgreesWithTryCreate<TPrimitive, TValue>(Gen<TValue> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull =>
        values.Sample(
            value =>
            {
                var accepted = TPrimitive.TryCreate(value, out var result, out var error);
                try
                {
                    var created = TPrimitive.Create(value);
                    return accepted && EqualityComparer<TPrimitive>.Default.Equals(created, result) && !error.IsError;
                }
                catch (DomainPrimitiveValidationException exception)
                {
                    return !accepted && exception.Error == error && error.IsError;
                }
            }
        );

    /// <summary>
    ///     Every value it accepts is written as text that parses back to the same domain primitive, and as JSON that reads back to it.
    /// </summary>
    private static void RoundTrips<TPrimitive, TValue>(Gen<TValue> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>, IParsable<TPrimitive>
        where TValue : notnull =>
        values.Sample(
            value =>
            {
                if (!TPrimitive.TryCreate(value, out var primitive, out _)) return true;
                var text = primitive.ToString()!;
                var parsed = TPrimitive.Parse(text, CultureInfo.InvariantCulture);
                var json = JsonSerializer.Deserialize<TPrimitive>(JsonSerializer.Serialize(primitive, Json), Json);
                return EqualityComparer<TPrimitive>.Default.Equals(parsed, primitive) && EqualityComparer<TPrimitive>.Default.Equals(json, primitive);
            }
        );

    /// <summary>
    ///     Normalizing a value it accepts again changes nothing.
    /// </summary>
    private static void NormalizesOnce<TPrimitive>(Gen<string> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, string> =>
        values.Sample(value => !TPrimitive.TryCreate(value, out var once, out _) || TPrimitive.Create(once.Value).Value == once.Value);

    [Test]
    public void Should_Create_What_TryCreate_Accepts()
    {
        CreateAgreesWithTryCreate<FirstName, string>(Text);
        CreateAgreesWithTryCreate<LastName, string>(Text);
        CreateAgreesWithTryCreate<DisplayName, string>(Text);
        CreateAgreesWithTryCreate<EmailAddress, string>(Text);
        CreateAgreesWithTryCreate<Score, int>(Gen.Int[-50, 150]);
        CreateAgreesWithTryCreate<Points, int>(Gen.Int);
        CreateAgreesWithTryCreate<Distance, double>(Gen.Double);
        CreateAgreesWithTryCreate<MovieRating, decimal>(Gen.Decimal[-1m, 6m]);
        CreateAgreesWithTryCreate<CustomerId, Guid>(Gen.Frequency((1, Gen.Const(Guid.Empty)), (9, Gen.Guid)));
        CreateAgreesWithTryCreate<OrderStatus, int>(Gen.Int[-1, 5]);
        CreateAgreesWithTryCreate<DeliveryStatus, int>(Gen.Int[-1, 5]);
        CreateAgreesWithTryCreate<WorkDay, DateOnly>(Gen.DateOnly);
        CreateAgreesWithTryCreate<OfficeHour, TimeOnly>(Gen.TimeOnly);
    }

    [Test]
    public void Should_Parse_What_It_Writes_And_Read_What_It_Writes()
    {
        RoundTrips<FirstName, string>(Text);
        RoundTrips<LastName, string>(Text);
        RoundTrips<Tag, string>(Text);
        RoundTrips<Score, int>(Gen.Int[0, 100]);
        RoundTrips<Points, int>(Gen.Int[0, 100]);
        RoundTrips<Distance, double>(Gen.Double[0, 1000]);
        RoundTrips<Measurement, double>(Gen.Double);
        RoundTrips<CompletionRate, decimal>(Gen.Decimal[0m, 100m]);
        RoundTrips<MovieRating, decimal>(Gen.Int[0, 500].Select(static z => z / 100m));
        RoundTrips<CustomerId, Guid>(Gen.Guid);
        RoundTrips<OrderNumber, int>(Gen.Int);
        RoundTrips<TransactionId, long>(Gen.Long);
        RoundTrips<Sku, string>(Text);
        RoundTrips<OrderStatus, int>(Gen.Int[1, 3]);
        RoundTrips<DeliveryStatus, int>(Gen.Int[1, 3]);
        RoundTrips<Size, string>(Gen.Int[0, 3].Select(static z => z switch { 0 => "s", 1 => "m", 2 => "xl", _ => "xxl" }));
        RoundTrips<CenturyDay, DateOnly>(Gen.DateOnly);
        RoundTrips<OfficeHour, TimeOnly>(Gen.TimeOnly);
        RoundTrips<Moment, DateTimeOffset>(Gen.DateTimeOffset);
        RoundTrips<ReportDay, LocalDate>(Gen.DateOnly.Select(static z => LocalDate.FromDateOnly(z)));
        RoundTrips<RecordedAt, Instant>(Gen.Long[-1_000_000_000_000L, 1_000_000_000_000L].Select(static z => Instant.FromUnixTimeMilliseconds(z)));
    }

    [Test]
    public void Should_Normalize_Once()
    {
        NormalizesOnce<FirstName>(Text);
        NormalizesOnce<DisplayName>(Text);
        NormalizesOnce<Tag>(Text);
        NormalizesOnce<Shout>(Text);
        NormalizesOnce<EmailAddress>(Text);
        NormalizesOnce<ArticleSlug>(Text);
        NormalizesOnce<Country>(Gen.String[Gen.Char["usUScaCA "], 0, 4]);
    }

    [Test]
    public void Should_Keep_The_Result_Of_An_Operator_In_Range()
    {
        Gen.Select(Gen.Int[0, 100], Gen.Int[0, 100], Gen.Int[-3, 3])
           .Sample(
                static (a, b, factor) =>
                {
                    var left = Points.Create(a);
                    var right = Points.Create(b);
                    foreach (var operation in new Func<Points>[] { () => left + right, () => left - right, () => left * factor, () => -left })
                    {
                        try
                        {
                            var result = operation();
                            if (result.Value is < 0 or > 100) return false;
                        }
                        catch (DomainPrimitiveValidationException exception) when (exception.Error.Code == "RANGE")
                        {
                            // a result out of range is rejected
                        }
                    }

                    return true;
                }
            );
    }

    [Test]
    public void Should_Order_Like_Its_Value()
    {
        Gen.Select(Gen.Int[0, 100], Gen.Int[0, 100])
           .Sample(static (a, b) => Math.Sign(Score.Create(a).CompareTo(Score.Create(b))) == Math.Sign(a.CompareTo(b)) && Score.Create(a) < Score.Create(b) == a < b);
        Gen.Select(Text, Text)
           .Sample(
                static (a, b) => !LastName.TryCreate(a, out var left, out _)
                              || !LastName.TryCreate(b, out var right, out _)
                              || Math.Sign(left.CompareTo(right)) == Math.Sign(string.CompareOrdinal(left.Value, right.Value))
            );
    }

    [Test]
    public void Should_Keep_A_Range_Ordered()
    {
        Gen.Select(Gen.DateOnly, Gen.DateOnly)
           .Sample(
                static (a, b) =>
                {
                    var accepted = Stay.TryCreate(a, b, out var stay, out _);
                    return accepted == a <= b && ( !accepted || stay.Contains(a) && stay.Contains(b) && stay.Days == b.DayNumber - a.DayNumber + 1 );
                }
            );
    }
}
