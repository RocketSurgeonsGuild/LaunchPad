namespace Primitives.Tests.Generator;

/// <summary>
///     String domain primitives: <c>[StringPrimitive]</c> with its rules, and the string kinds, as a struct and as a class.
/// </summary>
public class StringPrimitiveGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_String_Primitive()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, Trim, NotEmpty, MinLength(2), MaxLength(40), Regex("^[A-Za-z ]+$", ErrorCode = "LETTERS", ErrorMessage = "Only letters.")]
            public readonly partial record struct FirstName;

            [StringPrimitive, Trim, NotEmpty, MinLength(2), MaxLength(40), Regex("^[A-Za-z ]+$", ErrorCode = "LETTERS", ErrorMessage = "Only letters.")]
            public sealed partial record LastName;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    // A [GeneratedRegex] member of a string primitive or a string strong ID is a rule after its [Regex] rules, a property or a method.
    // Its pattern describes it, with the options that change what it matches inline.
    [Test]
    public async Task Should_Match_The_GeneratedRegex_Members()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using System.Text.RegularExpressions;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, Trim, Regex("^[A-Z]")]
            public readonly partial record struct Sku
            {
                [GeneratedRegex("^[A-Z]{3}-[0-9]{4}$", RegexOptions.CultureInvariant, 1000)]
                private static partial Regex Format { get; }

                [GeneratedRegex("[0-9]$")]
                private static partial Regex EndsWithDigit();

                // Described with its options inline; its name keeps the field of the [Regex] from being Pattern0.
                [GeneratedRegex("^[a-z]", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
                private static partial Regex Pattern0 { get; }
            }

            [StrongId<string>]
            public sealed partial record OrderNumber
            {
                [GeneratedRegex("^ORD-[0-9]+$")]
                private static partial Regex Format { get; }
            }
            """,
            regex: true
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_The_Normalization_Rules()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, TrimStart, TrimEnd, NormalizeWhitespace, LowerCase, ExactLength(8, ErrorCode = "SIZE", ErrorMessage = "Eight characters.")]
            public readonly partial record struct Tag;

            [StringPrimitive, UpperCase, Length(1, 10), Normalize<Shout>, CustomValidator<NoX>]
            public sealed partial record Shouted;

            public sealed class Shout : INormalizer<string>
            {
                public static string Normalize(string value) => value + "!";
            }

            public sealed class NoX : ICustomValidator<string>
            {
                public static PrimitiveError Validate(string value) => value.Contains('X') ? new("NO_X", "No X.") : PrimitiveError.None;
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_The_String_Kinds()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Email]
            public readonly partial record struct EmailAddress;

            [Url(AllowedSchemes = ["https", "ftp"], ErrorCode = "URL")]
            public sealed partial record Website;

            [CountryCode]
            public readonly partial record struct Country;

            [CurrencyCode(ErrorMessage = "Not a currency.")]
            public sealed partial record Currency;

            [IBAN]
            public readonly partial record struct BankAccount;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
