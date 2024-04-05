//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial record PatchRocket
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string?> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
#pragma warning disable CA1034
        public record Changes
        {
            public bool SerialNumber { get; init; }
            public bool Type { get; init; }
            public bool PlannedDate { get; init; }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Changes GetChangedState()
        {
            return new Changes()
            {
                SerialNumber = SerialNumber.HasBeenSet(),
                Type = Type.HasBeenSet(),
                PlannedDate = PlannedDate.HasBeenSet()
            };
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public global::Sample.Core.Operations.Rockets.Request ApplyChanges(global::Sample.Core.Operations.Rockets.Request state)
        {
            if (SerialNumber.HasBeenSet())
            {
                state = state with
                {
                    SerialNumber = SerialNumber!
                };
            }

            if (Type.HasBeenSet())
            {
                state = state with
                {
                    Type = Type!
                };
            }

            if (PlannedDate.HasBeenSet())
            {
                state = state with
                {
                    PlannedDate = PlannedDate!
                };
            }

            ResetChanges();
            return state;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public PatchRocket ResetChanges()
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(PlannedDate);
            return this;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        void IPropertyTracking<global::Sample.Core.Operations.Rockets.Request>.ResetChanges()
        {
            ResetChanges();
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public static global::Sample.Core.Operations.Rockets.PatchRocket TrackChanges(global::Sample.Core.Operations.Rockets.Request value) => new global::Sample.Core.Operations.Rockets.PatchRocket()
        {
            Id = value.Id,
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(value.SerialNumber),
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(value.PlannedDate)
        };
    }
}
#nullable restore
