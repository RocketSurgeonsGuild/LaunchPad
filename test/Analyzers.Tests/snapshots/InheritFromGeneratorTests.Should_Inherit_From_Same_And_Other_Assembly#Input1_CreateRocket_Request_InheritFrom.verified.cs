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
        public partial record Request
        {
            public string SerialNumber { get; set; }

            public Request With(Model value) => this with
            {
                SerialNumber = value.SerialNumber
            };
            public string Site { get; init; }

            public Request With(global::Sample.Contracts.LaunchContract value) => this with
            {
                Site = value.Site
            };
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
