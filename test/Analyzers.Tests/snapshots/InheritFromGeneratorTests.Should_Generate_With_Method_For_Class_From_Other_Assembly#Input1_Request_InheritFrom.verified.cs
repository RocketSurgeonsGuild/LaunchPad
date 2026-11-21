//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial class Request
    {
        public int Attempts;
        public string SerialNumber { get; set; }
        public global::System.DateTimeOffset? LaunchedAt { get; init; }
        public string? @class { get; set; }

        public Request With(global::Sample.Contracts.RocketContract value) => new Request
        {
            Id = this.Id,
            SerialNumber = value.SerialNumber,
            LaunchedAt = value.LaunchedAt,
            @class = value.@class,
            Attempts = value.Attempts
        };
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
