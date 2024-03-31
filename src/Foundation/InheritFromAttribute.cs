using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
[PublicAPI]
[Conditional("CodeGeneration")]
public sealed class InheritFromAttribute : Attribute
{
    /// <summary>
    ///     Create  the inherit from attribute
    /// </summary>
    /// <param name="classToCopy"></param>
    public InheritFromAttribute(Type classToCopy)
    {
    }

    /// <summary>
    /// Exclude the given properties from the copy
    /// </summary>
    public string[] Exclude { get; init; } = Array.Empty<string>();
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
    public InheritFromAttribute()
    {
    }

    /// <summary>
    /// Exclude the given properties from the copy
    /// </summary>
    public string[] Exclude { get; init; } = Array.Empty<string>();
}
