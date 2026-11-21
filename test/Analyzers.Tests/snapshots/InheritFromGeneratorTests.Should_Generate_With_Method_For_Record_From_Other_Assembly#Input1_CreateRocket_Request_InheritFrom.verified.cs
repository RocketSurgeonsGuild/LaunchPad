//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_CreateRocket_Request_InheritFrom.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Contracts;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record Request : global::Sample.Contracts.IHaveSerialNumber
        {
            [global::System.ComponentModel.DescriptionAttribute("The serial number"), global::System.ComponentModel.DataAnnotations.StringLengthAttribute(30, MinimumLength = 10)]
            public required string SerialNumber { get; set; }
            public string? Nickname { get; init; }

            [global::System.ComponentModel.DefaultValueAttribute(Sample.Contracts.RocketKind.Atlas)]
            public global::Sample.Contracts.RocketKind Kind { get; set; }
            public int? Stages { get; set; }
            public global::System.Collections.Immutable.ImmutableArray<string> Payloads { get; set; }

            public Request With(global::Sample.Contracts.RocketContract value) => this with
            {
                SerialNumber = value.SerialNumber,
                Nickname = value.Nickname,
                Kind = value.Kind,
                Stages = value.Stages,
                Payloads = value.Payloads
            };
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
