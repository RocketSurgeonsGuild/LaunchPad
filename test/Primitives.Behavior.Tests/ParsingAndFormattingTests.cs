using System.ComponentModel;
using System.Globalization;
using System.Text;

using static Primitives.Behavior.Tests.PrimitiveAssert;

namespace Primitives.Behavior.Tests;

public class ParsingTests
{
    [Test]
    public async Task Should_Parse_Text_Through_Its_Rules()
    {
        await Score.Parse("42", CultureInfo.InvariantCulture).Value.Should().BeEqualTo(42);
        await FirstName.Parse("  Ada  ", null).Value.Should().BeEqualTo("Ada");
        await LastName.Parse("Lovelace", null).Value.Should().BeEqualTo("Lovelace");
        await Score.TryParse("101", CultureInfo.InvariantCulture, out var outOfRange).Should().BeFalse();
        await outOfRange.IsDefault.Should().BeTrue();
        await Score.TryParse("forty", CultureInfo.InvariantCulture, out _).Should().BeFalse();
        await Score.TryParse((string?)null, CultureInfo.InvariantCulture, out _).Should().BeFalse();
        await LastName.TryParse((string?)null, null, out var nothing).Should().BeFalse();
        await nothing.Should().BeNull();
        await Throws<FormatException>(() => Score.Parse("101", CultureInfo.InvariantCulture));
    }

    [Test]
    public async Task Should_Parse_A_Span()
    {
        await Score.Parse("7".AsSpan(), CultureInfo.InvariantCulture).Value.Should().BeEqualTo(7);
        await FirstName.Parse(" Ada ".AsSpan(), null).Value.Should().BeEqualTo("Ada");
        await Score.TryParse("-1".AsSpan(), CultureInfo.InvariantCulture, out _).Should().BeFalse();
        var guid = Guid.CreateVersion7();
        await CustomerId.Parse(guid.ToString().AsSpan(), null).Value.Should().BeEqualTo(guid);
    }

    [Test]
    public async Task Should_Parse_Utf8_Text()
    {
        await Score.Parse("99"u8, CultureInfo.InvariantCulture).Value.Should().BeEqualTo(99);
        await EmailAddress.Parse("ADA@EXAMPLE.COM"u8, null).Value.Should().BeEqualTo("ada@example.com");
        await Score.TryParse("abc"u8, CultureInfo.InvariantCulture, out _).Should().BeFalse();
        await EmailAddress.TryParse("not an email"u8, null, out _).Should().BeFalse();
        await Throws<FormatException>(() => Score.Parse("1000"u8, CultureInfo.InvariantCulture));
        // a payload far past the maximum length is rejected
        await EmailAddress.TryParse(Encoding.UTF8.GetBytes(new string('a', 100_000)), null, out _).Should().BeFalse();
    }

    [Test]
    public async Task Should_Parse_With_The_Culture_It_Is_Given()
    {
        await Distance.Parse("12,5", CultureInfo.GetCultureInfo("de-DE")).Value.Should().BeEqualTo(12.5);
        await Distance.Parse("12.5", CultureInfo.InvariantCulture).Value.Should().BeEqualTo(12.5);
    }

    [Test]
    public async Task Should_Parse_Dates_And_Ids()
    {
        await CenturyDay.Parse("2026-11-23", CultureInfo.InvariantCulture).Value.Should().BeEqualTo(new DateOnly(2026, 11, 23));
        await OfficeHour.Parse("09:30:15", CultureInfo.InvariantCulture).Value.Should().BeEqualTo(new TimeOnly(9, 30, 15));
        await OrderNumber.Parse("42", CultureInfo.InvariantCulture).Value.Should().BeEqualTo(42);
        await OrderNumber.TryParse("0", CultureInfo.InvariantCulture, out _).Should().BeFalse();
        await CustomerId.TryParse(Guid.Empty.ToString(), null, out _).Should().BeFalse();
        await CustomerId.TryParse("not-a-guid", null, out _).Should().BeFalse();
    }
}

public class FormattingTests
{
    [Test]
    public async Task Should_Write_Its_Value()
    {
        await Score.Create(42).ToString().Should().BeEqualTo("42");
        await FirstName.Create("Ada").ToString().Should().BeEqualTo("Ada");
        await Distance.Create(12.5).ToString().Should().BeEqualTo("12.5");
        var guid = Guid.CreateVersion7();
        await CustomerId.Create(guid).ToString().Should().BeEqualTo(guid.ToString());
        await $"{CompletionRate.Create(12.5m):0.00}".Should().BeEqualTo("12.50");
        await Distance.Create(12.5).ToString("0.0", CultureInfo.GetCultureInfo("de-DE")).Should().BeEqualTo("12,5");
    }

    [Test]
    public async Task Should_Write_Dates_And_Times_As_Iso_8601()
    {
        // so that the text parses back to the same value: the general invariant format drops seconds and fractions
        await CenturyDay.Create(new DateOnly(2026, 11, 23)).ToString().Should().BeEqualTo("2026-11-23");
        var time = new TimeOnly(9, 30, 15, 250);
        await OfficeHour.Create(time).ToString().Should().BeEqualTo("09:30:15.2500000");
        await OfficeHour.Parse(OfficeHour.Create(time).ToString(), CultureInfo.InvariantCulture).Value.Should().BeEqualTo(time);
        var moment = new DateTimeOffset(2026, 11, 23, 10, 30, 15, 123, TimeSpan.FromHours(-5));
        await Moment.Create(moment).ToString().Should().BeEqualTo("2026-11-23T10:30:15.1230000-05:00");
        await Moment.Parse(Moment.Create(moment).ToString(), CultureInfo.InvariantCulture).Value.Should().BeEqualTo(moment);
    }

    [Test]
    public async Task Should_Format_Into_A_Span()
    {
        var buffer = new char[16];
        var written = Score.Create(42).TryFormat(buffer, out var count, default, CultureInfo.InvariantCulture);
        await written.Should().BeTrue();
        await new string(buffer, 0, count).Should().BeEqualTo("42");

        var tooSmall = FirstName.Create("Augusta").TryFormat(new char[3], out var none, default, null);
        await tooSmall.Should().BeFalse();
        await none.Should().BeEqualTo(0);

        var name = new char[16];
        await FirstName.Create("Ada").TryFormat(name, out var nameCount, default, null).Should().BeTrue();
        await new string(name, 0, nameCount).Should().BeEqualTo("Ada");

        // the default value writes nothing
        await default(Score).TryFormat(new char[4], out var empty, default, null).Should().BeTrue();
        await empty.Should().BeEqualTo(0);
    }

    [Test]
    public async Task Should_Format_Into_Utf8()
    {
        var bytes = new byte[32];
        await EmailAddress.Create("ada@example.com").TryFormat(bytes, out var count, default, null).Should().BeTrue();
        await Encoding.UTF8.GetString(bytes, 0, count).Should().BeEqualTo("ada@example.com");
        await Score.Create(7).TryFormat(bytes, out var scoreCount, default, CultureInfo.InvariantCulture).Should().BeTrue();
        await Encoding.UTF8.GetString(bytes, 0, scoreCount).Should().BeEqualTo("7");
        await EmailAddress.Create("ada@example.com").TryFormat(new byte[4], out var none, default, null).Should().BeFalse();
        await none.Should().BeEqualTo(0);
        await default(EmailAddress).TryFormat(bytes, out var empty, default, null).Should().BeTrue();
        await empty.Should().BeEqualTo(0);
    }
}

public class ComparisonTests
{
    [Test]
    public async Task Should_Order_Its_Values()
    {
        await ( Score.Create(1) < Score.Create(2) ).Should().BeTrue();
        await ( Score.Create(2) <= Score.Create(2) ).Should().BeTrue();
        await ( Points.Create(3) > Points.Create(2) ).Should().BeTrue();
        await FirstName.Create("Ada").CompareTo(FirstName.Create("Bea")).Should().BeLessThan(0);
        await Score.Create(5).CompareTo((object)Score.Create(5)).Should().BeEqualTo(0);
        await Score.Create(5).CompareTo(null).Should().BeEqualTo(1);
        await Throws<ArgumentException>(() => Score.Create(5).CompareTo("5"));
    }

    [Test]
    public async Task Should_Order_The_Default_Value_First()
    {
        // before any value, a negative one included
        await ( default(Counter) < Counter.Create(-100) ).Should().BeTrue();
        await default(Score).CompareTo(default).Should().BeEqualTo(0);
        await Score.Create(0).CompareTo(default).Should().BeEqualTo(1);
        var sorted = new[] { Score.Create(3), default, Score.Create(1) }.Order().ToArray();
        await sorted.SequenceEqual([default, Score.Create(1), Score.Create(3)]).Should().BeTrue();
    }

    [Test]
    public async Task Should_Order_Null_First()
    {
        await ( null < Points.Create(0) ).Should().BeTrue();
        await ( (Points?)null <= null ).Should().BeTrue();
        await Points.Create(0).CompareTo(null).Should().BeEqualTo(1);
        var sorted = new[] { Points.Create(3), null, Points.Create(1) }.Order().ToArray();
        await sorted.SequenceEqual([null, Points.Create(1), Points.Create(3)]).Should().BeTrue();
    }

    [Test]
    public async Task Should_Order_An_Offset_Time_By_Its_Instant()
    {
        var instant = NodaTime.Instant.FromUtc(2026, 11, 23, 12, 0);
        var toronto = RecordedAt.Create(instant);
        var later = OffsetMoment.Create(( instant + NodaTime.Duration.FromHours(1) ).WithOffset(NodaTime.Offset.FromHours(9)));
        var earlier = OffsetMoment.Create(instant.WithOffset(NodaTime.Offset.FromHours(-5)));

        await ( earlier < later ).Should().BeTrue();
        await toronto.CompareTo(RecordedAt.Create(instant)).Should().BeEqualTo(0);
    }
}

public class TypeConverterTests
{
    [Test]
    public async Task Should_Convert_From_Text_And_The_Value()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Score));

        await converter.CanConvertFrom(typeof(string)).Should().BeTrue();
        await converter.CanConvertFrom(typeof(int)).Should().BeTrue();
        await converter.ConvertFromInvariantString("7").Should().BeEqualTo(Score.Create(7));
        await converter.ConvertFrom(8).Should().BeEqualTo(Score.Create(8));
        await Throws<FormatException>(() => converter.ConvertFromInvariantString("700"));
        await Throws<Rocket.Surgery.LaunchPad.Primitives.DomainPrimitiveValidationException>(() => converter.ConvertFrom(700));
    }

    [Test]
    public async Task Should_Convert_To_Text_And_The_Value()
    {
        var converter = TypeDescriptor.GetConverter(typeof(CustomerId));
        var id = CustomerId.Create();

        await converter.CanConvertTo(typeof(Guid)).Should().BeTrue();
        await converter.ConvertToInvariantString(id).Should().BeEqualTo(id.Value.ToString());
        await converter.ConvertTo(id, typeof(Guid)).Should().BeEqualTo(id.Value);
    }

    [Test]
    public async Task Should_Convert_A_String_Primitive_And_A_Smart_Enum()
    {
        var names = TypeDescriptor.GetConverter(typeof(LastName));
        await names.ConvertFromInvariantString(" Lovelace ").Should().BeEqualTo(LastName.Create("Lovelace"));
        await names.ConvertToInvariantString(LastName.Create("Lovelace")).Should().BeEqualTo("Lovelace");

        var statuses = TypeDescriptor.GetConverter(typeof(OrderStatus));
        await statuses.ConvertFromInvariantString("completed").Should().BeEqualTo(OrderStatus.Completed);
        await statuses.ConvertFrom(2).Should().BeEqualTo(OrderStatus.Processing);
        await statuses.ConvertToInvariantString(OrderStatus.Pending).Should().BeEqualTo("Pending");
        await statuses.ConvertTo(OrderStatus.Pending, typeof(int)).Should().BeEqualTo(1);

        // a struct smart enum converts the same way, and a member converted from its value carries its data
        var deliveries = TypeDescriptor.GetConverter(typeof(DeliveryStatus));
        await deliveries.ConvertFromInvariantString("completed").Should().BeEqualTo(DeliveryStatus.Completed);
        await deliveries.ConvertFrom(2).Should().BeEqualTo(DeliveryStatus.Processing);
        await deliveries.ConvertToInvariantString(DeliveryStatus.Pending).Should().BeEqualTo("Pending");
        await deliveries.ConvertTo(DeliveryStatus.Pending, typeof(int)).Should().BeEqualTo(1);
        var sizes = TypeDescriptor.GetConverter(typeof(Size));
        await ( (Size)sizes.ConvertFromInvariantString("xl")! ).Label.Should().BeEqualTo("Extra large");
        await sizes.ConvertFromInvariantString("Medium").Should().BeEqualTo(Size.Medium);
        await sizes.ConvertToInvariantString(Size.ExtraLarge).Should().BeEqualTo("ExtraLarge");
        await sizes.ConvertTo(Size.Small, typeof(string)).Should().BeEqualTo("Small");
    }
}
