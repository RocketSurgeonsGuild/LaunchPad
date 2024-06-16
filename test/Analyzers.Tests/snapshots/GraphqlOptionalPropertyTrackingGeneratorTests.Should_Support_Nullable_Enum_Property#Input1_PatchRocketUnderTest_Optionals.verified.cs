//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_PatchRocketUnderTest_Optionals.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System;
using Sample.Core.Operations.Rockets;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial record PatchRocketUnderTest
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Guid Id { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public HotChocolate.Optional<string?> SerialNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public HotChocolate.Optional<RocketType?> Type { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public global::Sample.Core.Operations.Rockets.PatchRocket Create()
        {
            var value = new global::Sample.Core.Operations.Rockets.PatchRocket()
            {
                Id = Id
            };
            if (SerialNumber.HasValue)
            {
                value = value with
                {
                    SerialNumber = SerialNumber.Value
                };
            }

            if (Type.HasValue)
            {
                value = value with
                {
                    Type = Type.Value
                };
            }

            if (PlannedDate.HasValue)
            {
                value = value with
                {
                    PlannedDate = PlannedDate.Value
                };
            }

            return value;
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
