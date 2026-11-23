namespace Primitives.Tests.Generator;

/// <summary>
///     What the domain primitive generator does for every domain primitive: the attributes it writes, nested types, the project's settings,
///     and the diagnostics for what it can't complete.
/// </summary>
public class DomainPrimitiveGeneratorTests
{
    [Test]
    public async Task Should_Write_The_Attributes()
    {
        // With NodaTime the DatePrimitiveKind has its NodaTime members: the attributes compile in both projects.
        var withoutNodaTime = await GeneratorTestBuilder.Create(withAttributes: true)
                                                        .AddSources("namespace Sample { public class Library; }")
                                                        .Build()
                                                        .GenerateAsync(TestContext.Current!.Execution.CancellationToken);
        await withoutNodaTime.ShouldCompileCleanly();

        var withNodaTime = await GeneratorTestBuilder.Create(nodaTime: true, withAttributes: true)
                                                     .AddSources("namespace Sample { public class Library; }")
                                                     .Build()
                                                     .GenerateAsync(TestContext.Current!.Execution.CancellationToken);
        await withNodaTime.ShouldCompileCleanly();

        await Verify(withoutNodaTime);
    }

    [Test]
    public async Task Should_Complete_A_Nested_Domain_Primitive()
    {
        // Inside its containing types, records among them, and in the global namespace.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample
            {
                public static partial class Catalog
                {
                    public partial record Product
                    {
                        [Slug]
                        public readonly partial record struct ProductSlug;
                    }
                }
            }

            [Username]
            public readonly partial record struct GlobalUser;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Apply_The_Project_Settings()
    {
        // LaunchPadPrimitivesTrim, NotEmpty and MaxLength apply to a string primitive with no rules of its own; ExceptionType replaces the
        // exception Create throws.
        var result = await GeneratorTestBuilder.Create()
                                               .AddGlobalOption("build_property.LaunchPadPrimitivesTrim", "true")
                                               .AddGlobalOption("build_property.LaunchPadPrimitivesNotEmpty", "true")
                                               .AddGlobalOption("build_property.LaunchPadPrimitivesMaxLength", "64")
                                               .AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", "Sample.DomainException")
                                               .AddSources(
                                                    """
                                                    using Rocket.Surgery.LaunchPad.Primitives;

                                                    namespace Sample;

                                                    public sealed class DomainException(string message) : System.Exception(message);

                                                    [StringPrimitive]
                                                    public readonly partial record struct Note;

                                                    [StringPrimitive, MaxLength(10)]
                                                    public sealed partial record ShortNote;
                                                    """
                                                )
                                               .Build()
                                               .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Report_An_Unsupported_Value_Type()
    {
        // LPAD2101: a strong ID of a double, a number the generator has no category for, and a NodaTime kind without NodaTime.
        var result = await GeneratorTestBuilder.Create()
                                               .AddPreprocessorSymbol("LAUNCHPAD_PRIMITIVES_NODATIME")
                                               .AddSources(
                                                    """
                                                    using Rocket.Surgery.LaunchPad.Primitives;

                                                    namespace Sample;

                                                    [StrongId<double>]
                                                    public readonly partial record struct Measurement;

                                                    [NumericPrimitive<System.Numerics.BigInteger>]
                                                    public sealed partial record Huge;

                                                    [DatePrimitive(Kind = DatePrimitiveKind.Instant)]
                                                    public readonly partial record struct Moment;
                                                    """
                                                )
                                               .Build()
                                               .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_More_Than_One_Primitive_Attribute()
    {
        // LPAD2102: the type is left alone.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Email, Slug]
            public readonly partial record struct Contact;
            """
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_An_Invalid_Attribute_Argument()
    {
        // LPAD2103: a date kind that isn't a date, a Guid version other than 4 or 7, GuidVersion on an ID that isn't a Guid (reported, and the
        // ID is still completed), a date range of times, a Money named after one of its values, and a domain primitive nested in a generic type
        // (an attribute can't name a type nested in one, so its converters couldn't be declared).
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [BirthDate(Kind = DatePrimitiveKind.DateTimeOffset)]
            public readonly partial record struct Born;

            [StrongId<System.Guid>(GuidVersion = 5)]
            public readonly partial record struct OrderId;

            [StrongId<int>(GuidVersion = 4)]
            public readonly partial record struct LegacyId;

            [DateRange(Kind = DatePrimitiveKind.TimeOnly)]
            public readonly partial record struct Stay;

            public static partial class Ledger
            {
                [Money]
                public readonly partial record struct Amount;
            }

            public partial class Page<T>
            {
                [StrongId<int>]
                public sealed partial record PageId;
            }
            """
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Leave_A_Type_That_Is_Not_Partial_Alone()
    {
        // The shape of a declaration is the analyzers' to report: the generator writes nothing for it.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Email]
            public readonly record struct EmailAddress(string Value);
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
