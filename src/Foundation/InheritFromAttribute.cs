using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
/// </summary>
/// <remarks>
///     Create  the inherit from attribute
/// </remarks>
/// <param name="classToCopy"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
[PublicAPI]
[Conditional("CodeGeneration")]
public sealed class InheritFromAttribute(Type classToCopy) : Attribute
{

    /// <summary>
    ///     Exclude the given properties from the copy
    /// </summary>
    public string[] Exclude { get; init; } = [];
}

/// <summary>
///     Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
[PublicAPI]
[Conditional("CodeGeneration")]
public sealed class InheritFromAttribute<T> : Attribute where T : notnull
{
    /// <summary>
    ///     Create  the inherit from attribute
    /// </summary>
    public InheritFromAttribute() { }

    /// <summary>
    ///     Exclude the given properties from the copy
    /// </summary>
    public string[] Exclude { get; init; } = [];
}
