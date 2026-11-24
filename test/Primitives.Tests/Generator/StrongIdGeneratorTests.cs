namespace Primitives.Tests.Generator;

/// <summary>
///     Strong IDs of a Guid, an int, a long and a string.
/// </summary>
public class StrongIdGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Guid_Strong_Id()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StrongId<System.Guid>]
            public readonly partial record struct OrderId;

            [StrongId<System.Guid>(GuidVersion = 4)]
            public sealed partial record CustomerId;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Number_Strong_Id()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StrongId<int>]
            public readonly partial record struct LegacyId;

            [StrongId<long>(RejectEmpty = false)]
            public sealed partial record TransactionId;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_String_Strong_Id()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StrongId<string>, MaxLength(20)]
            public readonly partial record struct Sku;

            [StrongId<string>, Trim, UpperCase, Regex("^[A-Z0-9-]+$")]
            public sealed partial record PartNumber;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
