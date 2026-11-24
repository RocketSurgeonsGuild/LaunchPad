namespace Primitives.Tests.Generator;

/// <summary>
///     Date and time domain primitives: <c>[DatePrimitive]</c> and the date kinds, on the BCL types and on NodaTime's.
/// </summary>
public class DatePrimitiveGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Date_Primitive()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [DatePrimitive(PastOnly = true)]
            public readonly partial record struct PastDay;

            [DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset, FutureOnly = true, ErrorCode = "WHEN")]
            public sealed partial record Deadline;

            [DatePrimitive(Kind = DatePrimitiveKind.TimeOnly), PrimitiveRange("09:00", "17:00")]
            public readonly partial record struct OfficeHour;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_The_Date_Kinds()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [BirthDate(MaxAge = 120)]
            public readonly partial record struct Born;

            [ExpirationDate]
            public sealed partial record Expires;

            [BusinessDate]
            public readonly partial record struct WorkDay;

            [BusinessDate(AllowWeekends = true)]
            public sealed partial record AnyDay;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_NodaTime_Date_Primitive()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
            public readonly partial record struct HappenedAt;

            [DatePrimitive(Kind = DatePrimitiveKind.ZonedDateTime, FutureOnly = true)]
            public sealed partial record Appointment;

            [DatePrimitive(Kind = DatePrimitiveKind.LocalDate), PrimitiveRange("2000-01-01", "2100-12-31")]
            public readonly partial record struct ReportDay;

            [DatePrimitive(Kind = DatePrimitiveKind.Period)]
            public sealed partial record Term;
            """,
            nodaTime: true
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_The_NodaTime_Date_Kinds()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [BirthDate(Kind = DatePrimitiveKind.LocalDate)]
            public readonly partial record struct Born;

            [ExpirationDate(Kind = DatePrimitiveKind.LocalDate)]
            public sealed partial record Expires;

            [BusinessDate(Kind = DatePrimitiveKind.LocalDate)]
            public readonly partial record struct WorkDay;

            [DatePrimitive(Kind = DatePrimitiveKind.YearMonth, PastOnly = true)]
            public sealed partial record BillingMonth;
            """,
            nodaTime: true
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
