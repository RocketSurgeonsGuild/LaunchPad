// C# 15's union marker types, which .NET 11 ships, for this .NET 10 fixture. LaunchPad's own union types (RocketSurgeonsGuild/LaunchPad#3147)
// will replace this.
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal sealed class UnionAttribute : Attribute;

internal interface IUnion
{
    object? Value { get; }
}
