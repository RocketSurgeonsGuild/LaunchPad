//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_CreateRocket_Request_InheritFrom.cs
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static class CreateRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record Request
        {
            public string SerialNumber { get; set; }

            public Request With(Model value) => this with
            {
                SerialNumber = value.SerialNumber
            };
        }
    }
}
#nullable restore
