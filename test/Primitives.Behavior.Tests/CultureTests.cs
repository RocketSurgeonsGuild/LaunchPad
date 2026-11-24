using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Primitives.Behavior.Tests;

/// <summary>
///     Text is read and written in the invariant culture unless a culture is given: a null provider is the invariant culture, whatever the
///     current culture is, so text written on one machine parses on another.
/// </summary>
public class CultureTests
{
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    private static async Task InGerman(Func<Task> test)
    {
        var current = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = German;
        try
        {
            // the current culture writes a decimal comma
            await 2.5m.ToString(CultureInfo.CurrentCulture).Should().BeEqualTo("2,5");
            await test();
        }
        finally
        {
            CultureInfo.CurrentCulture = current;
        }
    }

    [Test]
    public Task Should_Round_Trip_A_Decimal_In_The_Invariant_Culture() => InGerman(
        async () =>
        {
            var weight = Weight.Create(2.5m);
            await weight.ToString().Should().BeEqualTo("2.5");
            await weight.ToString(null, null).Should().BeEqualTo("2.5");
            await $"{weight}".Should().BeEqualTo("2.5");
            await Weight.Parse("2.5", null).Should().BeEqualTo(weight);
            await Weight.Parse(weight.ToString(), null).Should().BeEqualTo(weight);
            await Weight.TryParse("2.5".AsSpan(), null, out var span).Should().BeTrue();
            await span.Should().BeEqualTo(weight);
            await Weight.TryParse("2.5"u8, null, out var utf8).Should().BeTrue();
            await utf8.Should().BeEqualTo(weight);
            await CompletionRate.Parse(CompletionRate.Create(12.5m).ToString(), null).Value.Should().BeEqualTo(12.5m);
            // the culture given is used
            await Weight.Parse("2,5", German).Should().BeEqualTo(weight);
            await weight.ToString(null, German).Should().BeEqualTo("2,5");
        }
    );

    [Test]
    public Task Should_Round_Trip_A_Double_In_The_Invariant_Culture() => InGerman(
        async () =>
        {
            var distance = Distance.Create(12.5);
            await distance.ToString().Should().BeEqualTo("12.5");
            await distance.ToString(null, null).Should().BeEqualTo("12.5");
            await $"{distance}".Should().BeEqualTo("12.5");
            await Distance.Parse("12.5", null).Should().BeEqualTo(distance);
            await Distance.TryParse("12.5", null, out var text).Should().BeTrue();
            await text.Should().BeEqualTo(distance);
            await Distance.TryParse("12.5".AsSpan(), null, out var span).Should().BeTrue();
            await span.Should().BeEqualTo(distance);
            await Distance.TryParse("12.5"u8, null, out var utf8).Should().BeTrue();
            await utf8.Should().BeEqualTo(distance);
            var buffer = new char[16];
            await distance.TryFormat(buffer, out var written, default, null).Should().BeTrue();
            await new string(buffer, 0, written).Should().BeEqualTo("12.5");
            // the culture given is used
            await Distance.Parse("12,5", German).Should().BeEqualTo(distance);
            await distance.ToString(null, German).Should().BeEqualTo("12,5");
        }
    );

    [Test]
    public Task Should_Round_Trip_A_Date_In_The_Invariant_Culture() => InGerman(
        async () =>
        {
            var day = CenturyDay.Create(new DateOnly(2026, 11, 23));
            await day.ToString().Should().BeEqualTo("2026-11-23");
            await CenturyDay.Parse(day.ToString(), null).Should().BeEqualTo(day);
            await CenturyDay.TryParse("2026-11-23".AsSpan(), null, out var span).Should().BeTrue();
            await span.Should().BeEqualTo(day);
            await CenturyDay.TryParse("2026-11-23"u8, null, out var utf8).Should().BeTrue();
            await utf8.Should().BeEqualTo(day);
            // German text is read only in the culture given
            await CenturyDay.TryParse("23.11.2026", null, out _).Should().BeFalse();
            await CenturyDay.Parse("23.11.2026", German).Should().BeEqualTo(day);

            var moment = Moment.Create(new DateTimeOffset(2026, 11, 23, 10, 30, 15, 123, TimeSpan.FromHours(1)));
            await Moment.Parse(moment.ToString(), null).Should().BeEqualTo(moment);
            await Moment.TryParse(moment.ToString(), null, out var parsed).Should().BeTrue();
            await parsed.Should().BeEqualTo(moment);
        }
    );

    [Test]
    public async Task Should_Convert_Text_In_The_Culture_The_Type_Converter_Is_Given()
    {
        var distances = TypeDescriptor.GetConverter(typeof(Distance));
        await distances.ConvertFrom(null, German, "12,5").Should().BeEqualTo(Distance.Create(12.5));
        await distances.ConvertTo(null, German, Distance.Create(12.5), typeof(string)).Should().BeEqualTo("12,5");
        await distances.ConvertFrom(null, CultureInfo.InvariantCulture, "12.5").Should().BeEqualTo(Distance.Create(12.5));
        await distances.ConvertTo(null, CultureInfo.InvariantCulture, Distance.Create(12.5), typeof(string)).Should().BeEqualTo("12.5");

        var weights = TypeDescriptor.GetConverter(typeof(Weight));
        await weights.ConvertFrom(null, German, "2,5").Should().BeEqualTo(Weight.Create(2.5m));
        await weights.ConvertTo(null, German, Weight.Create(2.5m), typeof(string)).Should().BeEqualTo("2,5");

        // a date is written in ISO 8601 in every culture, and read in the culture given
        var days = TypeDescriptor.GetConverter(typeof(CenturyDay));
        await days.ConvertFrom(null, German, "23.11.2026").Should().BeEqualTo(CenturyDay.Create(new DateOnly(2026, 11, 23)));
        await days.ConvertTo(null, German, CenturyDay.Create(new DateOnly(2026, 11, 23)), typeof(string)).Should().BeEqualTo("2026-11-23");
    }

    [Test]
    public Task Should_Convert_Text_In_The_Invariant_Culture_When_None_Is_Given() => InGerman(
        async () =>
        {
            // no culture (null) is the invariant culture, not the current one
            var distances = TypeDescriptor.GetConverter(typeof(Distance));
            await distances.ConvertFrom(null, null, "12.5").Should().BeEqualTo(Distance.Create(12.5));
            await distances.ConvertTo(null, null, Distance.Create(12.5), typeof(string)).Should().BeEqualTo("12.5");
            await distances.ConvertToInvariantString(Distance.Create(12.5)).Should().BeEqualTo("12.5");
        }
    );
}

/// <summary>
///     A DateOnly, TimeOnly or DateTimeOffset domain primitive is written in ISO 8601 (the round trip format "O") by every way of writing it.
/// </summary>
public class IsoFormattingTests
{
    private static async Task WritesIso<T>(T primitive, string iso)
        where T : ISpanFormattable, IUtf8SpanFormattable
    {
        await primitive.ToString().Should().BeEqualTo(iso);
        await primitive.ToString(null, null).Should().BeEqualTo(iso);
        await primitive.ToString("", CultureInfo.GetCultureInfo("de-DE")).Should().BeEqualTo(iso);
        await $"{primitive}".Should().BeEqualTo(iso);

        var chars = new char[64];
        await primitive.TryFormat(chars, out var charCount, default, null).Should().BeTrue();
        await new string(chars, 0, charCount).Should().BeEqualTo(iso);

        var bytes = new byte[64];
        await primitive.TryFormat(bytes, out var byteCount, default, null).Should().BeTrue();
        await Encoding.UTF8.GetString(bytes, 0, byteCount).Should().BeEqualTo(iso);
    }

    [Test]
    public async Task Should_Write_A_DateOnly_In_Iso_8601()
    {
        await WritesIso(CenturyDay.Create(new DateOnly(2026, 11, 23)), "2026-11-23");
        await WritesIso(PastDay.Create(new DateOnly(2001, 2, 3)), "2001-02-03");
    }

    [Test]
    public async Task Should_Write_A_TimeOnly_In_Iso_8601()
    {
        await WritesIso(OfficeHour.Create(new TimeOnly(9, 30, 15, 250)), "09:30:15.2500000");
        await WritesIso(OfficeHour.Create(new TimeOnly(9, 0)), "09:00:00.0000000");
    }

    [Test]
    public async Task Should_Write_A_DateTimeOffset_In_Iso_8601()
    {
        var moment = new DateTimeOffset(2026, 11, 23, 10, 30, 15, 123, TimeSpan.FromHours(-5));
        await WritesIso(Moment.Create(moment), "2026-11-23T10:30:15.1230000-05:00");
        await WritesIso(GlobalTimestamp.Create(new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.Zero)), "2001-02-03T04:05:06.0000000+00:00");
    }

    [Test]
    public async Task Should_Write_In_Iso_8601_Under_Any_Current_Culture()
    {
        var current = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        try
        {
            await WritesIso(CenturyDay.Create(new DateOnly(2026, 11, 23)), "2026-11-23");
            await WritesIso(OfficeHour.Create(new TimeOnly(9, 30, 15)), "09:30:15.0000000");
            await WritesIso(Moment.Create(new DateTimeOffset(2026, 11, 23, 10, 30, 15, TimeSpan.FromHours(1))), "2026-11-23T10:30:15.0000000+01:00");
        }
        finally
        {
            CultureInfo.CurrentCulture = current;
        }
    }

    [Test]
    public async Task Should_Use_A_Format_It_Is_Given()
    {
        await CenturyDay.Create(new DateOnly(2026, 11, 23)).ToString("d", CultureInfo.GetCultureInfo("de-DE")).Should().BeEqualTo("23.11.2026");
        await $"{CenturyDay.Create(new DateOnly(2026, 11, 23)):yyyy'/'MM}".Should().BeEqualTo("2026/11");
    }
}
