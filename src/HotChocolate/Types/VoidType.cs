using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a void or unit type in Hot Chocolate
/// </summary>
public class VoidType : ScalarType
{
    /// <summary>
    ///     THe constructor
    /// </summary>
    public VoidType() : base("Void", BindingBehavior.Implicit)
    {
    }

    /// <inheritdoc />
    public override Type RuntimeType { get; } = typeof(void);

    /// <inheritdoc />
    public override bool IsInstanceOfType(IValueNode valueSyntax)
    {
        return false;
    }

    /// <inheritdoc />
    public override object? ParseLiteral(IValueNode valueSyntax)
    {
        return new object();
    }

    /// <inheritdoc />
    public override IValueNode ParseValue(object? runtimeValue)
    {
        return new ObjectValueNode();
    }

    /// <inheritdoc />
    public override IValueNode ParseResult(object? resultValue)
    {
        return new ObjectValueNode();
    }

    /// <inheritdoc />
    public override bool TrySerialize(object? runtimeValue, out object? resultValue)
    {
        resultValue = new object();
        return true;
    }

    /// <inheritdoc />
    public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
    {
        runtimeValue = new object();
        return true;
    }
}
