//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_PatchGraphRocket_Optionals.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System;
using NodaTime;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial record PatchGraphRocket
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public Guid Id { get; set; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public HotChocolate.Optional<string?> SerialNumber { get; set; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public HotChocolate.Optional<int?> Type { get; set; }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public global::PatchRocket Create()
    {
        var value = new global::PatchRocket()
        {
            Id = Id
        };
        if (PlannedDate.HasValue)
        {
            value = value with
            {
                PlannedDate = PlannedDate.Value ?? default
            };
        }

        if (SerialNumber.HasValue)
        {
            value = value with
            {
                SerialNumber = SerialNumber.Value
            };
        }

        if (Type.HasValue)
        {
            value = value with
            {
                Type = Type.Value ?? default
            };
        }

        return value;
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
