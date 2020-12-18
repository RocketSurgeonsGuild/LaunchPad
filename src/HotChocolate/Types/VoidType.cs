using HotChocolate.Language;
using HotChocolate.Types;
using System;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    public class VoidType : ScalarType
    {
        public VoidType() : base("Void", BindingBehavior.Implicit) { }
        public override bool IsInstanceOfType(IValueNode valueSyntax) => false;

        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true) => null;

        public override IValueNode ParseValue(object? runtimeValue) => new NullValueNode(null);

        public override IValueNode ParseResult(object? resultValue) => new NullValueNode(null);

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;
            return true;
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;
            return true;
        }

        public override Type RuntimeType { get; } = typeof(void);
    }
}