//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_CreateRocket_Request_InheritFrom.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record Request
        {
            public Request With(Model value) => this with
            {
                SerialNumber = value.SerialNumber
            };
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
