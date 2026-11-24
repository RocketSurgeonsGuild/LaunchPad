namespace Primitives.Tests.Generator;

/// <summary>
///     Value objects: <c>[ValueObject]</c> and the Money, DateRange and TimeRange kinds.
/// </summary>
public class ValueObjectGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Value_Object()
    {
        // A value object holds plain values, nullable ones and other domain primitives (written in JSON as they are on their own).
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
    public async Task Should_Write_A_Smart_Enum_Of_Another_Assembly_As_Its_Name()
    {
        // A smart enum of a referenced assembly, whose attributes aren't compiled, is found by the TryFromName the generator wrote into it: the
        // value object writes it as its member's name and reads it by name or value, as its own converter does, for both forms.
        var core = await GeneratorTestBuilder
                        .Create()
                        .WithProjectName("Sample.Core")
                        .AddSources(
                             """
                             #nullable enable
                             using Rocket.Surgery.LaunchPad.Primitives;

                             namespace Sample.Core
                             {
                                 [SmartEnum<int>]
                                 public closed partial class Status
                                 {
                                     public static readonly Status Active = new Active_(1);
                                     public static readonly Status Closed = new Closed_(2);
                                 }

                                 [SmartEnum<string>]
                                 public readonly partial record struct Tier
                                 {
                                     public static readonly Tier Free = new("free");
                                     public static readonly Tier Paid = new("paid");
                                 }
                             }
                             """
                         )
                        .Build()
                        .GenerateAsync(TestContext.Current!.Execution.CancellationToken);
        await core.ShouldCompileCleanly();

        var result = await GeneratorTestBuilder
                          .Create()
                          .AddCompilationReferences(core)
                          .AddSources(
                               """
                               #nullable enable
                               using Rocket.Surgery.LaunchPad.Primitives;

                               namespace Sample
                               {
                                   [ValueObject]
                                   public sealed partial record Account
                                   {
                                       public required Sample.Core.Status Status { get; init; }
                                       public Sample.Core.Tier? Tier { get; init; }
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        var account = result.FinalCompilation.SyntaxTrees.Single(static z => Path.GetFileName(z.FilePath) == "Sample.Account.g.cs").ToString();
        await account.Should().Contain("global::Sample.Core.Status.TryFromName(reader.GetString(), true, out var StatusNamed)");
        await account.Should().Contain("global::Sample.Core.Tier.TryFromName(reader.GetString(), true, out var TierNamed)");
        await account.Should().Contain("writer.WriteStringValue(global::System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value.Status.Name));");
        await account.Should().Contain("if (value.Tier.Value.IsDefault) writer.WriteNullValue(); else writer.WriteStringValue(global::System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value.Tier.Value.Name));");
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
