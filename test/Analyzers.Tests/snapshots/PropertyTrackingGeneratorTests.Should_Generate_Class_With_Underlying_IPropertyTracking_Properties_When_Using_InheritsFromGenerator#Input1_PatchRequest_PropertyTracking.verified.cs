﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class PatchRequest
{
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

#pragma warning disable CA1034
    public record Changes
    {
        public bool Type { get; init; }
        public bool SerialNumber { get; init; }
    }

    public Changes GetChangedState()
    {
        return new Changes()
        {
            Type = Type.HasBeenSet(),
            SerialNumber = SerialNumber.HasBeenSet()
        };
    }

    public global::Request ApplyChanges(global::Request state)
    {
        if (Type.HasBeenSet())
        {
            state.Type = Type!;
        }

        if (SerialNumber.HasBeenSet())
        {
            state.SerialNumber = SerialNumber!;
        }

        ResetChanges();
        return state;
    }

    public PatchRequest ResetChanges()
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
        return this;
    }

    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    public static global::PatchRequest TrackChanges(global::Request value, global::System.Guid id) => new global::PatchRequest()
    {
        Id = id,
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber)
    };
}
#nullable restore
