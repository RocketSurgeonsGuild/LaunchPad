//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRequest_PropertyTracking.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using Sample.Contracts;
using System;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchRequest
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<int>))]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<string>))]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<string>))]
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string?> Nickname { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(default);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public record Changes
    {
        public bool Type { get; init; }
        public bool SerialNumber { get; init; }
        public bool Nickname { get; init; }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Changes GetChangedState()
    {
        return new Changes()
        {
            Type = Type.HasValue,
            SerialNumber = SerialNumber.HasValue,
            Nickname = Nickname.HasValue
        };
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public global::Request ApplyChanges(global::Request state)
    {
        if (Type.HasValue)
        {
            state.Type = Type!;
        }

        if (SerialNumber.HasValue)
        {
            state.SerialNumber = SerialNumber!;
        }

        if (Nickname.HasValue)
        {
            state.Nickname = Nickname!;
        }

        ResetChanges();
        return state;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public PatchRequest ResetChanges()
    {
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
        Nickname = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(Nickname);
        return this;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public static global::PatchRequest TrackChanges(global::Request value, global::System.Guid id) => new global::PatchRequest()
    {
        Id = id,
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type),
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
        Nickname = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(value.Nickname)
    };
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
