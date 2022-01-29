using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
/// </summary>
/// <remarks>
///     This attribute works on record classes only.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
[Conditional("CodeGeneration")]
public class InheritFromAttribute : Attribute
{
    /// <summary>
    ///     Create  the inherit from attribute
    /// </summary>
    /// <param name="classToCopy"></param>
    public InheritFromAttribute(Type classToCopy)
    {
        ClassToCopy = classToCopy;
    }

    /// <summary>
    ///     The class that is being copied from
    /// </summary>
    public Type ClassToCopy { get; }
}
