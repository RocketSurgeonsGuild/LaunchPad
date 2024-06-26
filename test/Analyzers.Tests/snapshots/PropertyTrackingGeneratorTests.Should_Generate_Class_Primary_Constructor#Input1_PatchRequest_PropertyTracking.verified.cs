﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchRequest
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Something { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
#pragma warning disable CA1034
    public record Changes
    {
        public bool Type { get; init; }
        public bool Something { get; init; }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Changes GetChangedState()
    {
        return new Changes()
        {
            Type = Type.HasBeenSet(),
            Something = Something.HasBeenSet()
        };
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
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

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public PatchRequest ResetChanges()
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        Something = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Something);
        return this;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public static global::PatchRequest TrackChanges(global::Request value) => new global::PatchRequest(value.Id)
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
        Something = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Something)
    };
}
#nullable restore
