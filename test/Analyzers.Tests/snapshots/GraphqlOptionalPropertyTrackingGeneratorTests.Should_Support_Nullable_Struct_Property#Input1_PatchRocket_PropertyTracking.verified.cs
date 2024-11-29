//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public partial record PatchRocket
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant?> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant?>.Empty(default);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
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
                SerialNumber = SerialNumber.HasValue,
                Type = Type.HasValue,
                PlannedDate = PlannedDate.HasValue
            };
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public global::Sample.Core.Operations.Rockets.Request ApplyChanges(global::Sample.Core.Operations.Rockets.Request state)
        {
            if (SerialNumber.HasValue)
            {
                state = state with
                {
                    SerialNumber = SerialNumber!
                };
            }

            if (Type.HasValue)
            {
                state = state with
                {
                    Type = Type!
                };
            }

            if (PlannedDate.HasValue)
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
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant?>.Empty(PlannedDate);
            return this;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        void IPropertyTracking<global::Sample.Core.Operations.Rockets.Request>.ResetChanges()
        {
            ResetChanges();
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public static global::Sample.Core.Operations.Rockets.PatchRocket TrackChanges(global::Sample.Core.Operations.Rockets.Request value) => new global::Sample.Core.Operations.Rockets.PatchRocket(value.Id)
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant?>.Empty(value.PlannedDate)
        };
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
