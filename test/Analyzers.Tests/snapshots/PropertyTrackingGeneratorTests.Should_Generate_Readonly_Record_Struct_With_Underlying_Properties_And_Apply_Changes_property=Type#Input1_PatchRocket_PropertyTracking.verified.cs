//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket_PropertyTracking.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
public readonly partial record struct PatchRocket
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<string>))]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get => field ?? Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default); init; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<int>))]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get => field ?? Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default); init; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public record Changes
    {
        public bool SerialNumber { get; init; }
        public bool Type { get; init; }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Changes GetChangedState()
    {
        return new Changes()
        {
            SerialNumber = SerialNumber.HasValue,
            Type = Type.HasValue
        };
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public global::Request ApplyChanges(global::Request state)
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

        return state;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public PatchRocket ResetChanges()
    {
        return this with
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber),
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type)
        };
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    void IPropertyTracking<global::Request>.ResetChanges()
    {
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public static global::PatchRocket TrackChanges(global::Request value) => new global::PatchRocket(value.Id)
    {
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type)
    };
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
