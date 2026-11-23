using NodaTime;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

public class ValueObjectTests
{
    [Test]
    public async Task Should_Create_A_Value_Object()
    {
        var address = Address.Create("1 Main St", "Springfield", Country.Create("US"), 3);

        await address.Street.Should().BeEqualTo("1 Main St");
        await address.City.Should().BeEqualTo("Springfield");
        await address.Country.Should().BeEqualTo(Country.Create("US"));
        await address.Floor.Should().BeEqualTo(3);
        await address.IsDefault.Should().BeFalse();
        await address.Should().BeEqualTo(Address.Create("1 Main St", "Springfield", Country.Create("US"), 3));
        await address.Should().NotBeEqualTo(Address.Create("2 Main St", "Springfield", Country.Create("US"), 3));
    }

    [Test]
    public async Task Should_Run_Its_Validation()
    {
        await Address.TryCreate(" ", null, Country.Create("US"), null, out var result, out var error).Should().BeFalse();
        await result.IsDefault.Should().BeTrue();
        await error.Code.Should().BeEqualTo("INVARIANT");
        await error.Message.Should().BeEqualTo("Street cannot be empty.");

        await Address.TryCreate("1 Main St", null, Country.Create("US"), -1, out _, out var floor).Should().BeFalse();
        await floor.Code.Should().BeEqualTo("FLOOR");

        var exception = await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => Address.Create("", null, Country.Create("US"), null));
        await exception!.Error.Code.Should().BeEqualTo("INVARIANT");
    }

    [Test]
    public async Task Should_Create_A_Class_Value_Object()
    {
        var contact = Contact.Create("Ada", EmailAddress.Create("ada@example.com"), null, null, new DateOnly(2020, 1, 1), new LocalDate(1815, 12, 10));

        await contact.Name.Should().BeEqualTo("Ada");
        await contact.Email.Should().BeEqualTo(EmailAddress.Create("ada@example.com"));
        await contact.Family.Should().BeNull();
        await contact.Country.Should().BeNull();
        await Contact.Description.Properties.Should().BeEquivalentTo(["Name", "Email", "Family", "Country", "Since", "Birthday"], CollectionOrdering.Matching);
        await Address.Description.Type.Should().BeEqualTo(typeof(Address));
        await Contact.PrimitiveName.Should().BeEqualTo("Contact");
    }
}

public class NestedPrimitiveTests
{
    [Test]
    public async Task Should_Reject_A_Struct_Primitive_Without_A_Value()
    {
        // default(Country) holds no country code: the value object can't hold it, as it couldn't be written or read back
        await Address.TryCreate("1 Main St", null, default, null, out var result, out var error).Should().BeFalse();
        await result.IsDefault.Should().BeTrue();
        await error.Code.Should().BeEqualTo("INVARIANT");
        await error.Message.Should().BeEqualTo("Address.Country must have a value.");
        var exception = await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => Address.Create("x", null, default, null));
        await exception!.Error.Code.Should().BeEqualTo("INVARIANT");
    }

    [Test]
    public async Task Should_Reject_A_Null_Class_Primitive()
    {
        await Household.TryCreate(null!, CountryClass.Create("GB"), out var result, out var family).Should().BeFalse();
        await result.Should().BeNull();
        await family.Code.Should().BeEqualTo("INVARIANT");
        await family.Message.Should().BeEqualTo("Household.Family must have a value.");
        await Household.TryCreate(LastName.Create("Lovelace"), null!, out _, out var country).Should().BeFalse();
        await country.Message.Should().BeEqualTo("Household.Country must have a value.");
        await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => Household.Create(null!, null!));
        await Household.Create(LastName.Create("Lovelace"), CountryClass.Create("GB")).Family.Should().BeEqualTo(LastName.Create("Lovelace"));
    }

    [Test]
    public async Task Should_Accept_A_Nullable_Primitive_Left_Null()
    {
        // struct (EmailAddress?, Country?) and class (LastName?) domain primitives that are nullable may be left out
        var contact = Contact.Create("Ada", null, null, null, new DateOnly(2020, 1, 2), null);

        await contact.Email.Should().BeNull();
        await contact.Family.Should().BeNull();
        await contact.Country.Should().BeNull();
        await Address.Create("1 Main St", null, Country.Create("US"), null).City.Should().BeNull();
    }
}

public class MoneyTests
{
    [Test]
    public async Task Should_Normalize_And_Validate_Its_Currency()
    {
        var cash = Cash.Create(10.5m, " usd ");

        await cash.Amount.Should().BeEqualTo(10.5m);
        await cash.Currency.Should().BeEqualTo("USD");
        await cash.ToString().Should().BeEqualTo("10.5 USD");
        await Cash.TryCreate(1m, "XYZ", out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("CURRENCY");
        await Cash.TryCreate(1m, null!, out _, out var missing).Should().BeFalse();
        await missing.Code.Should().BeEqualTo("CURRENCY");
    }

    [Test]
    public async Task Should_Add_Only_The_Same_Currency()
    {
        var sum = Cash.Create(10m, "USD") + Cash.Create(5.25m, "usd");

        await sum.Should().BeEqualTo(Cash.Create(15.25m, "USD"));
        await ( Cash.Create(10m, "USD") - Cash.Create(15m, "USD") ).Amount.Should().BeEqualTo(-5m);
        await ( Cash.Create(10m, "EUR") * 3 ).Should().BeEqualTo(Cash.Create(30m, "EUR"));
        await ( Cash.Create(10m, "EUR") / 4 ).Amount.Should().BeEqualTo(2.5m);
        await PrimitiveAssert.Throws<InvalidOperationException>(() => _ = Cash.Create(10m, "USD") + Cash.Create(10m, "EUR"));
    }

    [Test]
    public async Task Should_Compare_Only_The_Same_Currency()
    {
        await ( Cash.Create(10m, "USD") > Cash.Create(5m, "USD") ).Should().BeTrue();
        await ( Cash.Create(10m, "USD") <= Cash.Create(10m, "USD") ).Should().BeTrue();
        await PrimitiveAssert.Throws<InvalidOperationException>(() => _ = Cash.Create(10m, "USD") < Cash.Create(10m, "EUR"));
    }

    [Test]
    public async Task Should_Check_The_Result_Of_Its_Arithmetic()
    {
        await Price.TryCreate(-0.01m, "USD", out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("RANGE");
        await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => _ = Price.Create(5m, "USD") - Price.Create(6m, "USD"));
    }

    [Test]
    public async Task Should_Pin_Its_Currency()
    {
        await Dollars.Create(3m).Currency.Should().BeEqualTo("USD");
        await Dollars.TryCreate(3m, out var dollars, out _).Should().BeTrue();
        await dollars!.Amount.Should().BeEqualTo(3m);
        await Dollars.TryCreate(1m, "EUR", out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("CURRENCY");
        await error.Message.Should().BeEqualTo("Dollars must be in USD. Got 'EUR'.");
    }

    [Test]
    public async Task Should_Have_No_Value_When_Default()
    {
        await default(Cash).IsDefault.Should().BeTrue();
        await default(Cash).ToString().Should().BeEqualTo("0 ");
        await Cash.Create(0m, "USD").IsDefault.Should().BeFalse();
    }
}

public class RangeTests
{
    [Test]
    public async Task Should_Hold_A_Range_Of_Dates()
    {
        var stay = Stay.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));

        await stay.Days.Should().BeEqualTo(3);
        await stay.Contains(new DateOnly(2026, 1, 1)).Should().BeTrue();
        await stay.Contains(new DateOnly(2026, 1, 3)).Should().BeTrue();
        await stay.Contains(new DateOnly(2026, 1, 4)).Should().BeFalse();
        await stay.Overlaps(Stay.Create(new DateOnly(2026, 1, 3), new DateOnly(2026, 1, 9))).Should().BeTrue();
        await stay.Overlaps(Stay.Create(new DateOnly(2026, 1, 4), new DateOnly(2026, 1, 9))).Should().BeFalse();
        await stay.ToString().Should().BeEqualTo("2026-01-01/2026-01-03");
        await Stay.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 1)).Days.Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Reject_A_Range_That_Ends_Before_It_Starts()
    {
        await Stay.TryCreate(new DateOnly(2026, 1, 3), new DateOnly(2026, 1, 1), out _, out var error).Should().BeFalse();
        await error.Code.Should().BeEqualTo("RANGE");
        await error.Message.Should().BeEqualTo("Stay must not start after it ends.");
        await Shift.TryCreate(new LocalTime(17, 0), new LocalTime(9, 0), out _, out var shift).Should().BeFalse();
        await shift.Message.Should().BeEqualTo("A shift ends after it starts.");
    }

    [Test]
    public async Task Should_Hold_A_Range_Of_NodaTime_Values()
    {
        var semester = Semester.Create(new LocalDate(2026, 9, 1), new LocalDate(2026, 12, 20));
        await semester.Days.Should().BeEqualTo(111);
        await semester.Contains(new LocalDate(2026, 10, 1)).Should().BeTrue();
        await semester.ToString().Should().BeEqualTo("2026-09-01/2026-12-20");

        var shift = Shift.Create(new LocalTime(9, 0), new LocalTime(17, 30));
        await shift.Duration.Should().BeEqualTo(Duration.FromMinutes(510));
        await shift.ToString().Should().BeEqualTo("09:00:00/17:30:00");
    }

    [Test]
    public async Task Should_Hold_A_Range_Of_Times()
    {
        var hours = WorkShiftTime.Create(new TimeOnly(9, 0), new TimeOnly(17, 0));

        await hours.Duration.Should().BeEqualTo(TimeSpan.FromHours(8));
        await hours.Contains(new TimeOnly(12, 0)).Should().BeTrue();
        await hours.Overlaps(WorkShiftTime.Create(new TimeOnly(17, 0), new TimeOnly(18, 0))).Should().BeTrue();
        await hours.ToString().Should().BeEqualTo("09:00:00.0000000/17:00:00.0000000");
    }
}
