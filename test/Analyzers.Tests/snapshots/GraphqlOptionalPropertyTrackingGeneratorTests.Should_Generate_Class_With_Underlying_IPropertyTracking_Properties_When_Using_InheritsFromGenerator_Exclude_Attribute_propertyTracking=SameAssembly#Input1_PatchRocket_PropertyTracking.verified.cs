﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using NodaTime;
using System;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial class PatchRocket
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public record Changes
        {
            public bool PlannedDate { get; init; }
            public bool SerialNumber { get; init; }
            public bool Type { get; init; }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Changes GetChangedState()
        {
            return new Changes()
            {
                PlannedDate = PlannedDate.HasBeenSet(),
                SerialNumber = SerialNumber.HasBeenSet(),
                Type = Type.HasBeenSet()
            };
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public global::Request ApplyChanges(global::Request state)
        {
            if (PlannedDate.HasBeenSet())
            {
                state.PlannedDate = PlannedDate!;
            }

            if (SerialNumber.HasBeenSet())
            {
                state.SerialNumber = SerialNumber!;
            }

            if (Type.HasBeenSet())
            {
                state.Type = Type!;
            }

            ResetChanges();
            return state;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public PatchRocket ResetChanges()
        {
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(PlannedDate);
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
            return this;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        void IPropertyTracking<global::Request>.ResetChanges()
        {
            ResetChanges();
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public static global::Sample.Core.Operations.Rockets.PatchRocket TrackChanges(global::Request value, global::System.Guid id) => new global::Sample.Core.Operations.Rockets.PatchRocket()
        {
            Id = id,
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(value.PlannedDate),
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type)
        };
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
