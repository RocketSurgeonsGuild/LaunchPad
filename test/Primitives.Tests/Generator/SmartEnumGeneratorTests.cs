namespace Primitives.Tests.Generator;

/// <summary>
///     Smart enums of an int and of a string.
/// </summary>
public class SmartEnumGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Smart_Enum()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct OrderStatus
            {
                public static readonly OrderStatus Pending = new(1);
                public static readonly OrderStatus Shipped = new(2);
                public static readonly OrderStatus DeliveredToCustomer = new(3);
            }

            [SmartEnum<string>]
            public sealed partial record Role
            {
                public static readonly Role Admin = new("admin");
                public static readonly Role User = new("user");
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
