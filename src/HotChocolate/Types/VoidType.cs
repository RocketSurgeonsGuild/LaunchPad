using System.Text.Json;

using HotChocolate.Features;
using HotChocolate.Language;
using HotChocolate.Text.Json;
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
    public VoidType() : base("Void", BindingBehavior.Implicit) { }

    /// <inheritdoc />
    public override Type RuntimeType { get; } = typeof(void);

    /// <inheritdoc />
    public override ScalarSerializationType SerializationType => ScalarSerializationType.Object;

    /// <inheritdoc />
    public override bool IsValueCompatible(IValueNode valueLiteral) => false;

    /// <inheritdoc />
    public override bool IsValueCompatible(JsonElement inputValue) => false;

    /// <inheritdoc />
    public override object CoerceInputLiteral(IValueNode valueLiteral) => new();

    /// <inheritdoc />
    public override object CoerceInputValue(JsonElement inputValue, IFeatureProvider context) => new();

    /// <inheritdoc />
    public override void CoerceOutputValue(object runtimeValue, ResultElement resultValue) => resultValue.SetObjectValue(0);

    /// <inheritdoc />
    public override IValueNode ValueToLiteral(object runtimeValue) => new ObjectValueNode();
}
