using HotChocolate.Language;
using HotChocolate.Types;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers
{
    /// <summary>
    /// String to struct base type
    /// </summary>
    /// <typeparam name="TRuntimeType"></typeparam>
    public abstract class StringToStructBaseType<TRuntimeType> : ScalarType<TRuntimeType, StringValueNode>
        where TRuntimeType : struct
    {
        /// <summary>
        /// Create the base type
        /// </summary>
        /// <param name="name"></param>
        public StringToStructBaseType(string name) : base(name, bind: BindingBehavior.Implicit)
        {
        }
        /// <summary>
        /// Method to serialize
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected abstract string Serialize(TRuntimeType baseValue);
        /// <summary>
        /// Method to try and deserialize
        /// </summary>
        /// <param name="str"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected abstract bool TryDeserialize(string str, [NotNullWhen(true)] out TRuntimeType? output);

        /// <inheritdoc />
        protected override TRuntimeType ParseLiteral(StringValueNode literal)
        {
            if (TryDeserialize(literal.Value, out TRuntimeType? value))
            {
                return value.Value;
            }

            throw new SerializationException(
                $"Unable to deserialize string to {this.Name}",
                this);
        }

        /// <inheritdoc />
        protected override StringValueNode ParseValue(TRuntimeType value)
        {
            return new(Serialize(value));
        }

        /// <inheritdoc />
        public override IValueNode ParseResult(object? resultValue)
        {
            if (resultValue is null)
            {
                return NullValueNode.Default;
            }

            if (resultValue is string s)
            {
                return new StringValueNode(s);
            }

            if (resultValue is TRuntimeType v)
            {
                return ParseValue(v);
            }

            throw new SerializationException(
                $"Unable to deserialize string to {this.Name}",
                this);
        }

        /// <inheritdoc />
        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            if (runtimeValue is null)
            {
                resultValue = null;
                return true;
            }

            if (runtimeValue is TRuntimeType dt)
            {
                resultValue = Serialize(dt);
                return true;
            }

            resultValue = null;
            return false;
        }

        /// <inheritdoc />
        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            if (resultValue is null)
            {
                runtimeValue = null;
                return true;
            }

            if (resultValue is string str && TryDeserialize(str, out TRuntimeType? val))
            {
                runtimeValue = val;
                return true;
            }

            runtimeValue = null;
            return false;
        }
    }
}