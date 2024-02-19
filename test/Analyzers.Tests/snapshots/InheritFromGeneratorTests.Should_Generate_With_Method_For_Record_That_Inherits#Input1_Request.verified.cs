//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request.cs
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record Request
        {
            public Request With(Model value) => this with
            {
                SerialNumber = value.SerialNumber
            };
        }
    }
}
#nullable restore
