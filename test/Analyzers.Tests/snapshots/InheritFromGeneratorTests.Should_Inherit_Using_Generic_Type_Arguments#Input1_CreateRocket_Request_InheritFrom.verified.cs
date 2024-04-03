//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_CreateRocket_Request_InheritFrom.cs
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.Immutable;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record Request
        {
            public string SerialNumber { get; set; }
            public ImmutableArray<string> Items { get; set; }

            public Request With(Model value) => this with
            {
                SerialNumber = value.SerialNumber,
                Items = value.Items
            };
            public string OtherNumber { get; set; }

            public Request With(Other value) => this with
            {
                OtherNumber = value.OtherNumber
            };
        }
    }
}
#nullable restore
