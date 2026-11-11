using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a quantity: a count of items.
/// </summary>
/// <remarks>
///     Zero or more whole items.
/// </remarks>
public sealed class QuantityRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().IsPositiveOrZero();

    private QuantityRules() { }
}

/// <summary>A non-negative count of items, valid under <see cref="QuantityRules" />.</summary>
[StrongType<int, QuantityRules>]
public sealed partial class Quantity;
