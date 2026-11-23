namespace Primitives.Behavior.Tests;

[NumericPrimitive<int>, PrimitiveRange(0, 100)]
public readonly partial record struct Score;

[NumericPrimitive<int>(Operations = NumericOperations.All), PrimitiveRange(0, 100)]
public sealed partial record Points;

[NumericPrimitive<double>(Operations = NumericOperations.Addition | NumericOperations.ScalarMultiplication | NumericOperations.ScalarDivision), PrimitiveRange(0, 1000)]
public readonly partial record struct Distance;

[NumericPrimitive<int>(Operations = NumericOperations.Addition | NumericOperations.ScalarMultiplication | NumericOperations.Negation)]
public readonly partial record struct Counter;

[NumericPrimitive<long>(Operations = NumericOperations.Subtraction)]
public readonly partial record struct Ticks;

[NumericPrimitive<decimal>(Operations = NumericOperations.All), PrimitiveRange(0, 10, MinExclusive = true, MaxExclusive = true, ErrorCode = "OUT_OF_BOUNDS", ErrorMessage = "Between 0 and 10, exclusive.")]
public sealed partial record Weight;

[NumericPrimitive<double>]
public readonly partial record struct Measurement;

[NumericPrimitive<float>, PrimitiveRange(-1, 1)]
public readonly partial record struct Ratio;

[NumericPrimitive<byte>, PrimitiveRange(1, 300)]
public readonly partial record struct Level;

// The numeric kinds.

[Percentage]
public readonly partial record struct CompletionRate;

[Percentage(Min = 10, Max = 20)]
public sealed partial record Commission;

[Latitude]
public readonly partial record struct Latitude;

[Longitude]
public sealed partial record Longitude;

[Age]
public readonly partial record struct PersonAge;

[Rating(Min = 0, Max = 5, Scale = 2)]
public readonly partial record struct MovieRating;

[Month]
public readonly partial record struct MonthOfYear;

[Quarter]
public sealed partial record QuarterOfYear;

[Week]
public readonly partial record struct IsoWeek;

[Week(IsoWeekNumbering = false)]
public readonly partial record struct CalendarWeek;

[FiscalYear(MinYear = 2000, MaxYear = 2100)]
public readonly partial record struct CompanyFiscalYear;

[Quantity]
public readonly partial record struct ItemCount;

[Price]
public sealed partial record UnitPrice;

[Temperature]
public readonly partial record struct Celsius;
