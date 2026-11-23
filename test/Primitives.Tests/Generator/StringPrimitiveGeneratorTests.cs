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
