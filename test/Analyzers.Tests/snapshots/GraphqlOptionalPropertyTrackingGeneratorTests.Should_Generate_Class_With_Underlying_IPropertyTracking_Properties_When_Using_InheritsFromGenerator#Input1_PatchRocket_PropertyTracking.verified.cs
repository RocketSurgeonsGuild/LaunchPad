//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
using NodaTime;
using System;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial record PatchRocket
{
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

#pragma warning disable CA1034
    public record Changes
    {
        public bool PlannedDate { get; init; }
        public bool SerialNumber { get; init; }
        public bool Type { get; init; }
    }

    public Changes GetChangedState()
    {
        return new Changes()
        {
            PlannedDate = PlannedDate.HasBeenSet(),
            SerialNumber = SerialNumber.HasBeenSet(),
            Type = Type.HasBeenSet()
        };
    }

    public global::Request ApplyChanges(global::Request state)
    {
        if (PlannedDate.HasBeenSet())
        {
            state = state with
            {
                PlannedDate = PlannedDate!
            };
        }

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
        PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(PlannedDate);
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        return this;
    }

    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    public static global::PatchRocket TrackChanges(global::Request value, global::System.Guid id) => new global::PatchRocket()
    {
        Id = id,
        PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(value.PlannedDate),
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type)
    };
}
#nullable restore
