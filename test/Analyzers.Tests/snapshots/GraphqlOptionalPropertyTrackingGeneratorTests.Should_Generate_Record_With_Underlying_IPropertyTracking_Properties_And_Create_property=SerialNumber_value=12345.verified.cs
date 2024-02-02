//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Test2_PatchGraphRocket.cs
#nullable enable
using System;
using NodaTime;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial record PatchGraphRocket
{
    public HotChocolate.Optional<string?> SerialNumber { get; set; }
    public HotChocolate.Optional<int?> Type { get; set; }
    public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

    public global::Sample.Core.Operations.Rockets.PatchRocket Create()
    {
        var value = new global::Sample.Core.Operations.Rockets.PatchRocket()
        {
            Id = Id
        };
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
                Type = Type.Value
            };
        }

        if (PlannedDate.HasValue)
        {
            value = value with
            {
                PlannedDate = PlannedDate.Value
            };
        }

        return value;
    }
}
#nullable restore
