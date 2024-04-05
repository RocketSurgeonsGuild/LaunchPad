//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_CreateRocket_Request_InheritFrom.cs
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial class Request
        {
            public string SerialNumber { get; set; }

            public Request With(Model value) => new Request
            {
                Id = this.Id,
                SerialNumber = value.SerialNumber
            };
        }
    }
}
#nullable restore
