using HotChocolate.Language;
using HotChocolate.Types;
using System;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    /// <summary>
    /// Represents a void or unit type in Hot Chocolate
    /// </summary>
    public class VoidType : ScalarType
    {
        /// <summary>
        /// THe constructor
        /// </summary>
        public VoidType() : base("Void", BindingBehavior.Implicit) { }
        /// <inheritdoc />
        public override bool IsInstanceOfType(IValueNode valueSyntax) => false;

        /// <inheritdoc />
        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true) => null;

        /// <inheritdoc />
        public override IValueNode ParseValue(object? runtimeValue) => new NullValueNode(null);

        /// <inheritdoc />
        public override IValueNode ParseResult(object? resultValue) => new NullValueNode(null);

        /// <inheritdoc />
        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;
            return true;
        }

        /// <inheritdoc />
        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;
            return true;
        }

        /// <inheritdoc />
        public override Type RuntimeType { get; } = typeof(void);
    }
}