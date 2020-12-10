using System;
using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    /// <summary>
    /// Creates a new class that is a mutable version of the given class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [Conditional("CodeGeneration")]
    public class MutableAttribute : Attribute
    {
        public string Prefix { get; }
        public string Suffix { get; }

        public MutableAttribute(string prefix = "", string suffix = "ViewModel")
        {
            Prefix = prefix;
            Suffix = suffix;
        }
    }
}