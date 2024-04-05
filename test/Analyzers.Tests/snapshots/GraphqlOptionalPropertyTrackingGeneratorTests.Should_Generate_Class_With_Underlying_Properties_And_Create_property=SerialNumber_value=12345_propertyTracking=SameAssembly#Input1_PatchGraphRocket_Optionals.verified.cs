//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_PatchGraphRocket_Optionals.cs
#nullable enable
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class PatchGraphRocket
    {
        public Guid Id { get; set; }
        public HotChocolate.Optional<string?> SerialNumber { get; set; }
        public HotChocolate.Optional<int?> Type { get; set; }
        public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

        public global::Sample.Core.Operations.Rockets.PatchRocket Create()
        {
            var value = new global::Sample.Core.Operations.Rockets.PatchRocket()
            {
                Id = Id
            };
            if (SerialNumber.HasValue)
            {
                value.SerialNumber = SerialNumber.Value;
            }

            if (Type.HasValue)
            {
                value.Type = Type.Value ?? default;
            }

            if (PlannedDate.HasValue)
            {
                value.PlannedDate = PlannedDate.Value ?? default;
            }

            return value;
        }
    }
}
#nullable restore
