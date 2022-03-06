//HintName: Rocket.Surgery.LaunchPad.Analyzers\Rocket.Surgery.LaunchPad.Analyzers.ControllerActionBodyGenerator\Attributes.cs

using System;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    [AttributeUsage(AttributeTargets.Method)]
    class CreatedAttribute : Attribute
    {
        public CreatedAttribute(string methodName){}
    }
    [AttributeUsage(AttributeTargets.Method)]
    class AcceptedAttribute : Attribute
    {
        public AcceptedAttribute(string methodName){}
    }
}