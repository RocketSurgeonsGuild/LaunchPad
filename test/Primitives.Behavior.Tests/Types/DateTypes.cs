using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

[DatePrimitive(PastOnly = true)]
public readonly partial record struct PastDay;

[DatePrimitive(FutureOnly = true)]
public sealed partial record FutureDay;

[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset, PastOnly = true)]
public readonly partial record struct GlobalTimestamp;

[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset)]
public sealed partial record Moment;

[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly, FutureOnly = true)]
public readonly partial record struct ShiftStartTime;

[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly), PrimitiveRange("09:00", "17:00")]
public sealed partial record OfficeHour;

[DatePrimitive, PrimitiveRange("2000-01-01", "2099-12-31", ErrorCode = "CENTURY")]
public readonly partial record struct CenturyDay;

[BirthDate(MaxAge = 120)]
public readonly partial record struct CustomerBirthDate;

[ExpirationDate]
public sealed partial record CreditCardExpiration;

[BusinessDate]
public readonly partial record struct WorkDay;

[BusinessDate(AllowWeekends = true)]
public readonly partial record struct AnyDay;

// NodaTime kinds.

[DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
public readonly partial record struct HappenedAt;

[DatePrimitive(Kind = DatePrimitiveKind.Instant)]
public sealed partial record RecordedAt;

[DatePrimitive(Kind = DatePrimitiveKind.ZonedDateTime, FutureOnly = true)]
public sealed partial record Appointment;

[DatePrimitive(Kind = DatePrimitiveKind.OffsetDateTime)]
public readonly partial record struct OffsetMoment;

[DatePrimitive(Kind = DatePrimitiveKind.LocalDate), PrimitiveRange("2000-01-01", "2100-12-31")]
public readonly partial record struct ReportDay;

[DatePrimitive(Kind = DatePrimitiveKind.LocalTime)]
public sealed partial record AlarmTime;

[DatePrimitive(Kind = DatePrimitiveKind.LocalDateTime)]
public readonly partial record struct Wallclock;

[DatePrimitive(Kind = DatePrimitiveKind.Duration)]
public readonly partial record struct Elapsed;

[DatePrimitive(Kind = DatePrimitiveKind.Period)]
public sealed partial record Term;

[DatePrimitive(Kind = DatePrimitiveKind.AnnualDate)]
public readonly partial record struct Anniversary;

[DatePrimitive(Kind = DatePrimitiveKind.YearMonth, PastOnly = true)]
public sealed partial record BillingMonth;

[DatePrimitive(Kind = DatePrimitiveKind.Offset)]
public readonly partial record struct UtcOffset;

[BirthDate(Kind = DatePrimitiveKind.LocalDate)]
public readonly partial record struct NodaBirthDate;

[ExpirationDate(Kind = DatePrimitiveKind.LocalDate)]
public sealed partial record NodaExpiration;

[BusinessDate(Kind = DatePrimitiveKind.LocalDate)]
public readonly partial record struct NodaWorkDay;
