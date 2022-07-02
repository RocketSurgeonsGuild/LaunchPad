//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator/Attributes.cs

using System;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Method)]
    sealed class CreatedAttribute : Attribute
    {
        public CreatedAttribute(string methodName){ MethodName = methodName; }
        public string MethodName { get; }
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Method)]
    sealed class AcceptedAttribute : Attribute
    {
        public AcceptedAttribute(string methodName){ MethodName = methodName; }
        public string MethodName { get; }
    }
}