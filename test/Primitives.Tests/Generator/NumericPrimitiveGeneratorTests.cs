namespace Primitives.Tests.Generator;

/// <summary>
///     Numeric domain primitives: <c>[NumericPrimitive&lt;T&gt;]</c> with its operators and range, and the numeric kinds.
/// </summary>
public class NumericPrimitiveGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Numeric_Primitive_With_Operators()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [NumericPrimitive<int>(Operations = NumericOperations.All), PrimitiveRange(0, 100)]
            public readonly partial record struct Score;

            [NumericPrimitive<double>(Operations = NumericOperations.Addition | NumericOperations.ScalarDivision), PrimitiveRange(0, 1000, MaxExclusive = true, ErrorCode = "TOO_FAR")]
            public sealed partial record Distance;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Numeric_Primitive_Without_Rules()
    {
        // No range: a long checks nothing; a decimal only its kind's rules.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [NumericPrimitive<long>]
            public readonly partial record struct Counter;

            [NumericPrimitive<decimal>(Operations = NumericOperations.Negation)]
            public sealed partial record Balance;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_The_Numeric_Kinds()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Percentage]
            public readonly partial record struct Completion;

            [Latitude]
            public sealed partial record Latitude;

            [Rating(Max = 10, Scale = 2)]
            public readonly partial record struct Stars;

            [Week(IsoWeekNumbering = false)]
            public sealed partial record WeekOfYear;

            [Age(Max = 120, ErrorMessage = "Not a human age.")]
            public readonly partial record struct PersonAge;
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
