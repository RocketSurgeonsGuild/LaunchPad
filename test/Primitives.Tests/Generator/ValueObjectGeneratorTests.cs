namespace Primitives.Tests.Generator;

/// <summary>
///     Value objects: <c>[ValueObject]</c> and the Money, DateRange and TimeRange kinds.
/// </summary>
public class ValueObjectGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Value_Object()
    {
        // A value object holds plain values, nullable ones and other domain primitives (written as their value in JSON).
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [CountryCode]
            public readonly partial record struct Country;

            [Email]
            public sealed partial record EmailAddress;

            [ValueObject]
            public readonly partial record struct Address
            {
                public string Street { get; init; }
                public string? City { get; init; }
                public Country Country { get; init; }
                public int? Floor { get; init; }

                static partial void Validate(ref Address value, ref PrimitiveError error)
                {
                    if (string.IsNullOrWhiteSpace(value.Street)) error = new("INVARIANT", "The street is required.");
                }
            }

            [ValueObject]
            public sealed partial record Contact
            {
                public required string Name { get; init; }
                public EmailAddress? Email { get; init; }
                public Country? Country { get; init; }
                public System.DateOnly Since { get; init; }
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_Money()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Money(Min = 0)]
            public readonly partial record struct Price;

            [Money("USD")]
            public sealed partial record Dollars;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Date_Range()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [DateRange]
            public readonly partial record struct Stay;

            [DateRange(Kind = DatePrimitiveKind.LocalDate)]
            public sealed partial record Semester;
            """,
            nodaTime: true
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Time_Range()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [TimeRange]
            public readonly partial record struct OpeningHours;

            [TimeRange(Kind = DatePrimitiveKind.LocalTime, ErrorMessage = "A shift ends after it starts.")]
            public sealed partial record Shift;
            """,
            nodaTime: true
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
