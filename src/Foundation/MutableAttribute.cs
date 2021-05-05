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
        /// <summary>
        /// The prefix
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// The suffix
        /// </summary>
        public string Suffix { get; }

        /// <summary>
        /// Create the given mutable object
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        public MutableAttribute(string prefix = "", string suffix = "ViewModel")
        {
            Prefix = prefix;
            Suffix = suffix;
        }
    }
}