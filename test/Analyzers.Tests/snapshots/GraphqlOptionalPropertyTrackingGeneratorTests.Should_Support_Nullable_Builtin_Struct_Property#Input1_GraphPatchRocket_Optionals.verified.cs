﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_GraphPatchRocket_Optionals.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial record GraphPatchRocket
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Guid Id { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Instant PlannedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public HotChocolate.Optional<string?> SerialNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public HotChocolate.Optional<int?> Type { get; set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public global::Sample.Core.Operations.Rockets.PatchRocket Create()
        {
            var value = new global::Sample.Core.Operations.Rockets.PatchRocket()
            {
                Id = Id,
                PlannedDate = PlannedDate
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

            return value;
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
