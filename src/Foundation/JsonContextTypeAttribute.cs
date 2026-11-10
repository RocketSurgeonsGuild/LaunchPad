namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.
/// </summary>
/// <remarks>
///     <para>
///         The generator that ships with Rocket.Surgery.LaunchPad.Foundation writes one for every mediator request of the assembly and its response
///         or stream item type, every <c>IPropertyTracking&lt;T&gt;</c> type and every type marked <see cref="IncludeInJsonContextAttribute" />.
///         An application's build reads them from its referenced assemblies and writes a <c>JsonSerializerContext</c> that declares them all.
///     </para>
///     <para>
///         The context is written for the referenced assemblies because their metadata is complete: the System.Text.Json source generator sees the
///         members other generators added to a type (property tracking, <c>[InheritFrom]</c>), which it cannot see for a type compiled in the
///         same project.
///     </para>
/// </remarks>
/// <param name="type">The type.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsonContextTypeAttribute(Type type) : Attribute
{
    /// <summary>The type.</summary>
    public Type Type { get; } = type;

    /// <summary>
    ///     The name of the type's property on the generated context. The build picks a unique name from the type's name when it isn't set.
    /// </summary>
    public string? TypeInfoPropertyName { get; set; }
}

/// <summary>
///     Puts a type in the JSON context LaunchPad generates in the applications that reference this assembly, for a type that crosses the wire as
///     JSON but isn't a mediator request or response.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
public sealed class IncludeInJsonContextAttribute : Attribute;
