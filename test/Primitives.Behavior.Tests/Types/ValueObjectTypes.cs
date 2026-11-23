using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

[ValueObject]
public readonly partial record struct Address
{
    public string Street { get; init; }
    public string? City { get; init; }
    public Country Country { get; init; }
    public int? Floor { get; init; }

    static void Validate(ref Address value, ref PrimitiveError error)
    {
        if (string.IsNullOrWhiteSpace(value.Street)) error = new("INVARIANT", "Street cannot be empty.");
        else if (value.Floor < 0) error = new("FLOOR", "Floor cannot be negative.");
    }
}

[ValueObject]
public sealed partial record Contact
{
    public required string Name { get; init; }
    public EmailAddress? Email { get; init; }
    public LastName? Family { get; init; }
    public Country? Country { get; init; }
    public DateOnly Since { get; init; }
    public NodaTime.LocalDate? Birthday { get; init; }
}

[ValueObject]
public readonly partial record struct GeoPoint
{
    public Latitude Latitude { get; init; }
    public Longitude Longitude { get; init; }
}

[Money]
public readonly partial record struct Cash;

[Money(Min = 0)]
public readonly partial record struct Price;

[Money("USD")]
public sealed partial record Dollars;

[DateRange]
public readonly partial record struct Stay;

[DateRange(Kind = DatePrimitiveKind.LocalDate)]
public sealed partial record Semester;

[TimeRange]
public readonly partial record struct WorkShiftTime;

[TimeRange(Kind = DatePrimitiveKind.LocalTime, ErrorMessage = "A shift ends after it starts.")]
public sealed partial record Shift;

// A class value object whose domain primitives are classes, neither of them nullable: a null one is missing.
[ValueObject]
public sealed partial record Household
{
    public required LastName Family { get; init; }
    public required CountryClass Country { get; init; }
}

// A struct value object of plain values only: its default value (row 0, column 0) is a value like any other.
[ValueObject]
public readonly partial record struct GridCell
{
    public int Row { get; init; }
    public int Column { get; init; }
}
