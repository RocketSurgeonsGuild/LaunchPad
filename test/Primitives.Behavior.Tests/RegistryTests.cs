using Rocket.Surgery.LaunchPad.Primitives.Generated;

namespace Primitives.Behavior.Tests;

/// <summary>
///     The registry the templates read (<c>LaunchPadPrimitives</c>), which the registry generator fills in.
/// </summary>
public class RegistryTests
{
    [Test]
    public async Task Should_Describe_The_Domain_Primitives_Of_The_Project()
    {
        await LaunchPadPrimitives.Find(typeof(CustomerId))!.IsStrongId.Should().BeTrue();
        await LaunchPadPrimitives.Find(typeof(CustomerId?))!.Type.Should().BeEqualTo(typeof(CustomerId));
        await LaunchPadPrimitives.Find(typeof(OrderStatus))!.IsSmartEnum.Should().BeTrue();
        // a struct smart enum is listed too, and found from its nullable type
        await LaunchPadPrimitives.Find(typeof(DeliveryStatus))!.IsSmartEnum.Should().BeTrue();
        await LaunchPadPrimitives.Find(typeof(Size?))!.Type.Should().BeEqualTo(typeof(Size));
        await LaunchPadPrimitives.Find(typeof(Size))!.AllowedValues.Should().BeEquivalentTo(["extraLarge", "medium", "small"], CollectionOrdering.Matching);
        await LaunchPadPrimitives.Find(typeof(EmailAddress))!.MaxLength.Should().BeEqualTo(320);
        await LaunchPadPrimitives.Find(typeof(Catalog.Product.NestedSlug)).Should().NotBeNull();
        await LaunchPadPrimitives.Find(typeof(string)).Should().BeNull();
        await LaunchPadPrimitives.Find(typeof(Stay)).Should().BeNull();
        await LaunchPadPrimitives.IsValueObject(typeof(Stay)).Should().BeTrue();
        await LaunchPadPrimitives.IsValueObject(typeof(Stay?)).Should().BeTrue();
        await LaunchPadPrimitives.IsValueObject(typeof(Contact)).Should().BeTrue();
        await LaunchPadPrimitives.IsValueObject(typeof(Score)).Should().BeFalse();
        await LaunchPadPrimitives.OpenApiRepresentation.Should().BeEqualTo("Component");
        await LaunchPadPrimitives.GraphQLRepresentation.Should().BeEqualTo("Value");
    }
}
