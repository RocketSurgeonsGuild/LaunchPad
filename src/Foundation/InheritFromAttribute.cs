using System;
using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    /// <summary>
    /// Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
    /// </summary>
    /// <remarks>
    /// This attribute works on record classes only.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [Conditional("CodeGeneration")]
    public class InheritFromAttribute : Attribute
    {
        public Type ClassToCopy { get; }

        public InheritFromAttribute(Type classToCopy)
        {
            ClassToCopy = classToCopy;
        }
    }
}