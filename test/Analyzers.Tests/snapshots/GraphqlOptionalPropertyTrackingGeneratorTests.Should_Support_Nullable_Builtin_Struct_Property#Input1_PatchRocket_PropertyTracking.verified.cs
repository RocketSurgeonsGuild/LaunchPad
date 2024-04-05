//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
using System;

namespace Sample.Core.Operations.Rockets
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial record PatchRocket
    {
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int?> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int?>.Empty(default);

#pragma warning disable CA1034
        public record Changes
        {
            public bool SerialNumber { get; init; }
            public bool Type { get; init; }
        }

        public Changes GetChangedState()
        {
            return new Changes()
            {
                SerialNumber = SerialNumber.HasBeenSet(),
                Type = Type.HasBeenSet()
            };
        }

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

            ResetChanges();
            return state;
        }

        public PatchRocket ResetChanges()
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int?>.Empty(Type);
            return this;
        }

        void IPropertyTracking<global::Sample.Core.Operations.Rockets.Request>.ResetChanges()
        {
            ResetChanges();
        }

        public static global::Sample.Core.Operations.Rockets.PatchRocket TrackChanges(global::Sample.Core.Operations.Rockets.Request value, global::NodaTime.Instant plannedDate) => new global::Sample.Core.Operations.Rockets.PatchRocket()
        {
            Id = value.Id,
            PlannedDate = plannedDate,
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int?>.Empty(value.Type)
        };
    }
}
#nullable restore
