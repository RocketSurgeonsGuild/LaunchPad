//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchRequest
{
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Something { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

#pragma warning disable CA1034
    public record Changes
    {
        public bool Type { get; init; }
        public bool Something { get; init; }
    }

    public Changes GetChangedState()
    {
        return new Changes()
        {
            Type = Type.HasBeenSet(),
            Something = Something.HasBeenSet()
        };
    }

    public global::Request ApplyChanges(global::Request state)
    {
        if (Type.HasBeenSet())
        {
            state.Type = Type!;
        }

        if (Something.HasBeenSet())
        {
            state.Something = Something!;
        }

        ResetChanges();
        return state;
    }

    public PatchRequest ResetChanges()
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        Something = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Something);
        return this;
    }

    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    public static global::PatchRequest TrackChanges(global::Request value) => new global::PatchRequest(value.Id)
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
        Something = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Something)
    };
}
#nullable restore
