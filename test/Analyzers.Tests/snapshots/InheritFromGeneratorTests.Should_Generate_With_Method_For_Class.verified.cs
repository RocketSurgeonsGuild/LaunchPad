//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Test0_Request.cs
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
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
