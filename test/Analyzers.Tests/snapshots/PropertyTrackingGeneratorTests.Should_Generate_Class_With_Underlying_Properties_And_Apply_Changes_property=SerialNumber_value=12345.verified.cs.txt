//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Test1_PatchRocket.cs
#nullable enable
using System;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchRocket
{
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);
#pragma warning disable CA1034
    public record Changes
    {
        public bool SerialNumber { get; init; }

        public bool Type { get; init; }
    }

    public Changes GetChangedState()
    {
        return new Changes()
        {SerialNumber = SerialNumber.HasBeenSet(), Type = Type.HasBeenSet()};
    }

    public global::Request ApplyChanges(global::Request state)
    {
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

    public PatchRocket ResetChanges()
    {
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        return this;
    }

    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }
}
#nullable restore
